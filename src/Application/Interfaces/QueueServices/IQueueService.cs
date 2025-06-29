namespace Application.Interfaces.QueueServices;

public interface IQueueService
{
    Task SendMessageAsync<T>(T serviceBusMessage, string queueName);
}
