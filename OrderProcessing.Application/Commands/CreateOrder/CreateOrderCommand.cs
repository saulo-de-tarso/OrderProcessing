using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Application.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    [Required]
    public string ClientId { get; set; } = default!;
    [Required]
    public List<string> Items { get; set; } = default!;
}
