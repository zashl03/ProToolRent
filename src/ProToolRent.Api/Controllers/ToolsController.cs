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
    [ProducesResponseType(typeof(ToolResponse), 200)]
    [ProducesResponseType(404)]
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

    [HttpGet("get-by-owner/{id:guid}")]
    [ProducesResponseType(typeof(ToolResponse), 200)]
    [ProducesResponseType(404)]
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

    [HttpPost]
    [ProducesResponseType(typeof(CreateToolCommand), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateToolRequest request)
    {
        var command = new CreateToolCommand(
            request.Brand, request.Name, request.Power,
            request.Description, request.Status,
            request.Price, request.CategoryId, request.UserId);

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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteToolCommand(id));

        return result.ErrorType switch
        {
            ErrorType.None => NoContent(),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error }) 
        };
    }
}
