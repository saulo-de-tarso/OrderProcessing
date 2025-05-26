using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Interfaces;

public interface IMessageBroker
{
    Task InitializeAsync();
    Task PublishOrderAsync(Order order);
    Task ConsumeOrdersAsync(Func<Order, Task> processOrder);

}
