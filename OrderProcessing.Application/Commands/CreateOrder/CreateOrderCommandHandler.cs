using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler(IOrderRepository orderRepository,
    IMessageBroker messageBroker,
    IMapper mapper,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = mapper.Map<Order>(command);
        order.Id = Guid.NewGuid();
        
        
        await orderRepository.AddAsync(order);
        await messageBroker.PublishOrderAsync(order);
        
        logger.LogInformation($"Order {order.Id} for client {order.ClientId} created and published to queue.");
        return order.Id;
    }

}
