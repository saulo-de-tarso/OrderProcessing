using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.Commands.CreateOrder;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Queries.GetOrderStatus;

namespace OrderProcessing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder(CreateOrderCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new {id}, null);
    }

    [HttpGet("id")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid id)
    {
        var order = await mediator.Send(new GetOrderStatusQuery(id));
        return Ok(order);
    }

}
