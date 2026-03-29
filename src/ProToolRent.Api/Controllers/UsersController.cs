using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Application.Commands.CreateUser;
using ProToolRent.Application.Common;
using ProToolRent.Application.Queries.GetUserById;
using ProToolRent.Domain.Enums;

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

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.PasswordHash,
            request.FirstName,
            request.LastName,
            request.City,
            request.Organization,
            request.Phone,
            Enum.Parse<UserRole>(request.Role));

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
