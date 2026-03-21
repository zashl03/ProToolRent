using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Application.Commands.CreateCategory;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetCategoryById;

namespace ProToolRent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));

        return result.ErrorType switch
        {
            ErrorType.None => Ok(CategoryResponse.FromDto(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCategoryCommand), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request)
    {
        var command = new CreateCategoryCommand(request.Name, request.ParentId);

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                new CreateCategoryResponse(result.Value!)),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
