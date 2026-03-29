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

    [Authorize(Roles = "Admin,Landlord")]
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

    [Authorize(Roles = "Admin,Landlord")]
    [HttpPost]
    [ProducesResponseType(typeof(CreateToolResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateToolRequest request)
    {
        var command = new CreateToolCommand(
            request.Brand, 
            request.Name, 
            request.Power,
            request.Description, 
            request.TotalQuantity,
            request.ReservedQuantity, 
            request.Price, 
            request.CategoryId,
            request.UserId
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
}
