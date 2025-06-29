
using System.Text.Json;
using Application.Interfaces.QueueServices;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace Infrastructure.Persistence.QueueServices;
public class QueueService : IQueueService
{
    private readonly ServiceBusClient _client;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly Dictionary<string, ServiceBusProcessor> _processors = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    public QueueService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        var connectionString = configuration["ServiceBusConnectionString"];
        _client = new ServiceBusClient(connectionString);
        _adminClient = new ServiceBusAdministrationClient(connectionString);
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    ///  This method sends a message to the specified Service Bus queue.   
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceBusMessage"></param>
    /// <param name="queueName"></param>
    /// <returns></returns>
    public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
    {
        await EnsureQueueExistsAsync(queueName);
        await using ServiceBusSender sender = _client.CreateSender(queueName);
        try
        {
            string messageBody = JsonSerializer.Serialize(serviceBusMessage);
            ServiceBusMessage message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }

    /// <summary>
    /// This method starts processing messages from the specified Service Bus queue.
    /// </summary>
    /// <param name="queueName"></param>
    /// <returns></returns>
    public async Task EnsureQueueExistsAsync(string queueName)
    {
        try
        {
            if (!await _adminClient.QueueExistsAsync(queueName))
            {
                await _adminClient.CreateQueueAsync(queueName);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queueName"></param>
    /// <param name="handleMessageAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartListeningAsync<T>(string queueName, Func<T, IServiceProvider, Task> handleMessageAsync, CancellationToken cancellationToken)
    {
        if (_processors.ContainsKey(queueName)) return;

        var processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async args =>
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                var message = JsonSerializer.Deserialize<T>(messageBody, _jsonOptions);
                if (message != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    await handleMessageAsync(message, scope.ServiceProvider);
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message handling failed: {ex}");
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine($"Processor error: {args.Exception.Message}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken);
        _processors[queueName] = processor;
    }

}
