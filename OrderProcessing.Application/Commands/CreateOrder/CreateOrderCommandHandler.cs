using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler(IOrderRepository orderRepository,
    IMessageQueue messageQueue,
    IMapper mapper,
    ILogger logger) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = mapper.Map<Order>(command);
        order.Id = Guid.NewGuid();
        orderRepository.Add(order);
        await messageQueue.PublishAsync(order.Id);
        logger.LogInformation($"Order {order.Id} created and published to queue.");
        return order.Id;
    }

}
