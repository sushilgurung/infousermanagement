namespace Application.Interfaces.QueueServices;
public interface IUserActionLogQueueConsumer
{
    Task StartProcessingAsync(CancellationToken cancellationToken=default);
    Task StopProcessingAsync();
}
