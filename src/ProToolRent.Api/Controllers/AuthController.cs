using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProToolRent.Api.Extensions;
using ProToolRent.Api.Contracts.Responses;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Application.Common;
using ProToolRent.Application.Authentication.Contracts;
using ProToolRent.Application.Authentication.Commands.Register;
using ProToolRent.Application.Authentication.Commands.Login;
using ProToolRent.Application.Authentication.Commands.Logout;
using ProToolRent.Application.Authentication.Commands.Refresh;

namespace ProToolRent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.RepeatPassword, request.Role);

        var result = await _mediator.Send(command);

        if (result.ErrorType != ErrorType.None)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => Unauthorized(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        Response.AppendRefreshToken(result.Value!.RefreshToken);

        return Ok(new AuthUserResponse
            (result.Value.AccessToken,
            result.Value.UserId,
            result.Value.Role));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await _mediator.Send(command);

        if (result.ErrorType != ErrorType.None)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => Unauthorized(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        Response.AppendRefreshToken(result.Value!.RefreshToken);

        return Ok(new AuthUserResponse
            (result.Value.AccessToken,
            result.Value.UserId,
            result.Value.Role));
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.GetRefreshToken();

        if (refreshToken == null)
            return Unauthorized();

        var command = new LogoutCommand(refreshToken);

        var result = await _mediator.Send(command);

        Response.DeleteRefreshToken();

        return result.ErrorType switch
        {
            ErrorType.None => NoContent(),
            _ => Unauthorized(new { error = result.Error })
        };
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.GetRefreshToken();

        if (refreshToken == null)
            return Unauthorized();

        var command = new RefreshCommand(refreshToken);

        var result = await _mediator.Send(command);

        if (result.ErrorType != ErrorType.None)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => Unauthorized(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
        }

        Response.AppendRefreshToken(refreshToken);

        return Ok(new AccessTokenResponse(result.Value!.AccessToken));
    }
}
