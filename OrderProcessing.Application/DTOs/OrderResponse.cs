namespace OrderProcessing.Application.DTOs;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
