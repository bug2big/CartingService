namespace Carting.Services.MessageConsumerService;

public interface IMessageConsumerService
{
    Task ReceiveMessageAsync();

    public void Close();
}
