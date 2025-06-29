using Application.Interfaces.QueueServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence.Extensions;
public static class ApplicationBuilderExtensions
{
    private static IUserActionLogQueueConsumer _userActionLogQueueReceiver { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        _userActionLogQueueReceiver = app.ApplicationServices.GetService<IUserActionLogQueueConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopping.Register(OnStop);
        return app;
    }
    private static void OnStop()
    {
        _userActionLogQueueReceiver.StopProcessingAsync();
    }

    private static void OnStart()
    {
        _userActionLogQueueReceiver.StartProcessingAsync();
    }
}

