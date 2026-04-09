using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Application.Commands.CreateOrder;
using ProToolRent.Application.Commands.CreateOrderItem;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetOrderById;
using ProToolRent.Application.Queries.GetOrderItemById;
using ProToolRent.Application.Queries.GetOrders;

namespace ProToolRent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {  
        _mediator = mediator; 
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);

        var result = await _mediator.Send(query);

        return result.ErrorType switch
        {
            ErrorType.None => Ok(OrderResponse.FromDto(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize(Roles = "Admin,Tenant")]
    [HttpPost]
    [ProducesResponseType(typeof(OrderSummaryResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        if(!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Пользователь не найден");

        var command = new CreateOrderCommand(
            userId, 
            request.ToolId, 
            request.StartDate, 
            request.EndDate, 
            request.Quantity);

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => Created($"/api/orders/{result.Value!.OrderId}",OrderSummaryResponse.FromDto(result.Value!)),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize(Roles = "Admin,Tenant")]
    [HttpPost("{orderId}/items")]
    [ProducesResponseType(typeof(CreateOrderItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrderItem(Guid orderId, [FromBody] CreateOrderItemRequest request)
    {
       var command = new CreateOrderItemCommand(
            request.OrderId,
            request.ToolId,
            request.StartDate,
            request.EndDate,
            request.Quantity
            );

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                new CreateOrderItemResponse(result.Value!)),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet("{orderId:guid}/items/{orderItemId:guid}")]
    [ProducesResponseType(typeof(OrderItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderItemById(Guid orderId, Guid orderItemId)
    {
        var query = new GetOrderItemByIdQuery(orderId, orderItemId);

        var result = await _mediator.Send(query);

        return result.ErrorType switch
        {
            ErrorType.None => Ok(OrderItemResponse.FromDto(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize]
    [HttpGet("my")]
    [ProducesResponseType(typeof(List<OrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMy()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if(!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();
        
        var command = new GetOrdersQuery(userId);
        
        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => Ok(OrderSummaryResponse.FromDtoList(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
