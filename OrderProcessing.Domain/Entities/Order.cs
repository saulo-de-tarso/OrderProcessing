using OrderProcessing.Domain.Enums;

namespace OrderProcessing.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public List<string> Items { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
