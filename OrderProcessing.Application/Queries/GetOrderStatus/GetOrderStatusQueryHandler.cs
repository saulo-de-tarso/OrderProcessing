using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Application.Queries.GetOrderStatus;

public class GetOrderStatusQueryHandler(IOrderRepository orderRepository,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetOrderStatusQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var order = orderRepository.GetById(request.OrderId);

        var response = mapper.Map<OrderResponse>(order);

        logger.LogInformation($"Order {order.Id} found with status: {order.Status}.");

        return await Task.FromResult(response);


    }

}
