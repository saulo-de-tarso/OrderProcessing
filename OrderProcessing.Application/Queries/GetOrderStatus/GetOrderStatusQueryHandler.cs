using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.Exceptions;
using System.Data;

namespace OrderProcessing.Application.Queries.GetOrderStatus;

public class GetOrderStatusQueryHandler(IOrderRepository orderRepository,
    IMapper mapper,
    ILogger<GetOrderStatusQueryHandler> logger) : IRequestHandler<GetOrderStatusQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId);

        if (order == null)
        {
            logger.LogWarning($"Order {request.OrderId} not found.");
            throw new NotFoundException(nameof(Order), request.OrderId.ToString());
        }

        var response = mapper.Map<OrderResponse>(order);

        logger.LogInformation($"Order {order.Id} found with status: {order.Status}.");

        return await Task.FromResult(response);

    }

}
