using MediatR;

namespace OrderProcessing.Application.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public string ClientId { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
}
