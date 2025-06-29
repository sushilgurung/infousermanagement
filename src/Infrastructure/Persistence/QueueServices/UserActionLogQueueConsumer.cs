
using System.Text.Json;
using Application.Interfaces.QueueServices;
using Azure.Messaging.ServiceBus;

namespace Infrastructure.Persistence.QueueServices;
public class UserActionLogQueueConsumer : IUserActionLogQueueConsumer
{
    private readonly IConfiguration _configuration;
    private ServiceBusProcessor _serviceBusProcessor;

    private readonly ServiceBusClient _client;

    private readonly IServiceScopeFactory _scopeFactory;

    public UserActionLogQueueConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        var subscriptionQueueName = _configuration.GetValue<string>("TopicAndQueueNames:UseractionlogsQueue");
        _client = new ServiceBusClient(serviceBusConnectionString);
        _serviceBusProcessor = _client.CreateProcessor(subscriptionQueueName);
        _scopeFactory = scopeFactory;
    }
    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
        await _serviceBusProcessor.StartProcessingAsync();
    }
    public async Task StopProcessingAsync()
    {
        await _serviceBusProcessor.StartProcessingAsync();
        await _serviceBusProcessor.DisposeAsync();
    }


    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        using var scope = _scopeFactory.CreateScope();
        var _userActionLogService = scope.ServiceProvider.GetRequiredService<IUserActionLogService>();

        string messageBody = args.Message.Body.ToString();
        UserActionLog receivedMessage = JsonSerializer.Deserialize<UserActionLog>(messageBody);
        try
        {
            await _userActionLogService.LogUserActionAsync(receivedMessage)
               .ConfigureAwait(false);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error: {args.Exception.Message}");
        return Task.CompletedTask;
    }

}
