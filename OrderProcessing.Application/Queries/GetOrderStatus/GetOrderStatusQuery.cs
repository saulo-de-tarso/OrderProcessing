using MediatR;
using OrderProcessing.Application.DTOs;

namespace OrderProcessing.Application.Queries.GetOrderStatus;

public class GetOrderStatusQuery : IRequest<OrderResponse>
{
    public Guid OrderId { get; set; }
}
