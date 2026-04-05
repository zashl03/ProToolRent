using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Application;
using ProToolRent.Application.Commands.CreateTool;
using ProToolRent.Application.Commands.DeleteTool;
using ProToolRent.Application.Queries.GetToolById;
using ProToolRent.Application.Queries.GetToolsByUserId;
using ProToolRent.Application.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProToolRent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ToolsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-by-id/{id:guid}")]
    [ProducesResponseType(typeof(ToolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetToolByIdQuery(id));

        return result.ErrorType switch
        {
            ErrorType.None => Ok(ToolResponse.FromDto(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize(Roles = "Admin,Landlord")]
    [HttpGet("get-by-owner/{id:guid}")]
    [ProducesResponseType(typeof(ToolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetToolsByUserAsync(Guid id)
    {
        var result = await _mediator.Send(new GetToolsByUserIdQuery(id));

        return result.ErrorType switch
        {
            ErrorType.None => Ok(ToolResponse.FromDtoList(result.Value!)),
            ErrorType.NotFound => NotFound(new {error = result.Error }),
            _ => BadRequest(new{ error = result.Error })
        };
    }

    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<ToolResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPagedTools([FromQuery] GetPagedToolsRequest request)
    {
        var result = await _mediator.Send(new GetPagedToolsQuery(request.PageNumber, request.PageSize));

        PagedResult<ToolResponse> response = new(
            result.Value!.Items.Select(ToolResponse.FromDto).ToList(),
            result.Value.TotalCount
        );
        return result.ErrorType switch
        {
            ErrorType.None => Ok(response),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize(Roles = "Admin,Landlord")]
    [HttpPost]
    [ProducesResponseType(typeof(CreateToolResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateToolRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { error = "Invalid user ID in token" });
        }

        var command = new CreateToolCommand(
            request.Brand, 
            request.Name, 
            request.Power,
            request.Description, 
            request.TotalQuantity,
            request.Price, 
            request.CategoryId,
            userId
            );

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                new CreateToolResponse(result.Value!)),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [Authorize(Roles = "Admin,Landlord")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteToolCommand(id);

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => NoContent(),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error }) 
        };
    }

    [Authorize(Roles = "Landlord")]
    [HttpGet("my")]
    [ProducesResponseType(typeof(PagedResult<ToolResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyTools([FromQuery] GetPagedToolsRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if(!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { error = "Invalid user ID in token" });
        }

        var result = await _mediator.Send(new GetToolsQuery(userId, request.PageNumber, request.PageSize));

        PagedResult<ToolResponse> response = new(
            result.Value!.Items.Select(ToolResponse.FromDto).ToList(),
            result.Value.TotalCount
        );
        return result.ErrorType switch
        {
            ErrorType.None => Ok(response),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
