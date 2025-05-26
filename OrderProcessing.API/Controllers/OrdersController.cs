using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.Commands.CreateOrder;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Queries.GetOrderStatus;
using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody]CreateOrderCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new {id}, null);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("id")]
    public async Task<ActionResult<OrderResponse>> GetOrder([FromQuery][Required]Guid id)
    {
        var order = await mediator.Send(new GetOrderStatusQuery(id));
        return Ok(order);
    }

}
