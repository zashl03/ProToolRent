using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Application.Commands.CreateUser;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetUserById;

namespace ProToolRent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));

        return result.ErrorType switch
        {
            ErrorType.None => Ok(UserResponse.FromDto(result.Value!)),
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserCommand), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand request)
    {
        var command = new CreateUserCommand(
            request.Fullname,
            request.Organization,
            request.City,
            request.RoleId);

        var result = await _mediator.Send(command);

        return result.ErrorType switch
        {
            ErrorType.None => CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                new CreateUserResponse(result.Value!)),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
