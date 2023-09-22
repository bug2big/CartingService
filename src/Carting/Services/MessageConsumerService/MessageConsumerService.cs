using Carting.Data.Repositories.Carts;
using Carting.Models;
using Carting.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Carting.Services.MessageConsumerService;

public class MessageConsumerService : IMessageConsumerService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ICartRepository _cartRepository;
    private readonly RabbitMqServiceSettings _rabbitMqServiceSettings;

    public MessageConsumerService(
        ICartRepository cartRepository,
        IOptions<RabbitMqServiceSettings> rabbitMqServiceSettingsOptions)
    {
        _cartRepository = cartRepository;
        _rabbitMqServiceSettings = rabbitMqServiceSettingsOptions.Value;
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqServiceSettings.HostName,
            UserName = _rabbitMqServiceSettings.UserName,
            Password = _rabbitMqServiceSettings.Password,
            VirtualHost = _rabbitMqServiceSettings.VirtualHost,
            Port = _rabbitMqServiceSettings.Port
        };

        _connection = connectionFactory.CreateConnection()!;
        _channel = _connection.CreateModel();
    }

    public Task ReceiveMessageAsync()
    {
        var queueName = _rabbitMqServiceSettings.QueueName;
        var routingKey = _rabbitMqServiceSettings.RoutingKey;
        _channel.ExchangeDeclare(queueName, ExchangeType.Topic, true, false, null);
        _channel.QueueDeclare(queueName, true, false, false, null);
        _channel.QueueBind(queueName, queueName, routingKey, null);
        _channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var updatedProductModel = JsonSerializer.Deserialize<UpdateProductModel>(message)!;

            var carts = (_cartRepository.GetCartItemsAsync(updatedProductModel.Id).Result)
                .ToList();

            foreach (var cartItem in carts)
            {
                cartItem.Price = updatedProductModel.Price ?? cartItem.Price;
                cartItem.Quantity = updatedProductModel.Amount ?? cartItem.Quantity;
                cartItem.Name = updatedProductModel.Name ?? cartItem.Name;
                cartItem.Image = updatedProductModel.Image ?? cartItem.Image;

                _cartRepository.UpsertAsync(cartItem);
            }

            _channel.BasicAck(args.DeliveryTag, false);
        };

        var consumerTags = _channel.BasicConsume(queueName, false, consumer);
        _channel.BasicCancel(consumerTags);

        return Task.CompletedTask;
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
