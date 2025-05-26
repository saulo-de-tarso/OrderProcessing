namespace OrderProcessing.Application.Interfaces;

public interface IMessageQueue
{
    Task PublishAsync(Guid orderId);
    void Consume(Action<Guid> process);
}
