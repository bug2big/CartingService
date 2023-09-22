using Carting.Services.MessageConsumerService;

namespace Carting.Extensions;

public static class ApplicationBuilderExtentions
{
    private static IMessageConsumerService _listener { get; set; } = null!;

    public static WebApplication UseRabbitListener(this WebApplication app)
    {
        _listener = app.Services.GetService<IMessageConsumerService>()!;

        var lifetime = app.Services.GetService<IHostApplicationLifetime>()!;
        lifetime.ApplicationStarted.Register(OnStarted);
        lifetime.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        _listener.ReceiveMessageAsync();
    }

    private static void OnStopping()
    {
        _listener.Close();
    }
}