using MediatR;
using OrderProcessing.Application.DTOs;

namespace OrderProcessing.Application.Queries.GetOrderStatus;

public class GetOrderStatusQuery(Guid orderId) : IRequest<OrderResponse>
{
    public Guid OrderId { get; set; } = orderId;
}
