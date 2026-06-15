using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProToolRent.Api.Contracts.Requests;
using ProToolRent.Api.Contracts.Responses;

namespace ProToolRent.IntegrationTests;

public class AuthApiTests : ApiTestBase
{
    public AuthApiTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WhenPasswordsMatch_ShouldReturnTokensAndRefreshCookie()
    {
        var context = await RegisterAsync();

        context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        context.AuthResponse.Should().NotBeNull();
        context.AuthResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        context.AuthResponse.Role.Should().Be("Tenant");
        context.RefreshCookie.Should().StartWith("RefreshToken=");
    }

    [Fact]
    public async Task Register_WhenPasswordsDoNotMatch_ShouldReturnBadRequest()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                $"user_{Guid.NewGuid()}@test.com",
                "Password123!",
                "Password1234!",
                "Tenant"),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnTokens()
    {
        var context = await RegisterAsync();

        var loginResponse = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(context.Email, context.Password),
            Ct);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await ReadResponseAsync<AuthUserResponse>(loginResponse);
        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        authResponse.UserId.Should().Be(context.AuthResponse!.UserId);
    }

    [Fact]
    public async Task Refresh_WhenRefreshCookieIsValid_ShouldReturnNewAccessToken()
    {
        var context = await RegisterAsync();

        var refreshResponse = await SendAuthRequestAsync(HttpMethod.Post, "/api/auth/refresh", context.RefreshCookie);

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokenResponse = await ReadResponseAsync<AccessTokenResponse>(refreshResponse);
        tokenResponse.Should().NotBeNull();
        tokenResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokenResponse.Role.Should().Be("Tenant");
        GetRefreshCookie(refreshResponse).Should().NotBe(context.RefreshCookie);
    }

    [Fact]
    public async Task Refresh_WhenRefreshCookieIsMissing_ShouldReturnUnauthorized()
    {
        var response = await Client.PostAsync("/api/auth/refresh", null, Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WhenRefreshCookieIsValid_ShouldReturnNoContent()
    {
        var context = await RegisterAsync();

        var logoutResponse = await SendAuthRequestAsync(HttpMethod.Post, "/api/auth/logout", context.RefreshCookie);

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteCookie = logoutResponse.Headers.GetValues("Set-Cookie")
            .Single(header => header.StartsWith("RefreshToken=", StringComparison.OrdinalIgnoreCase));
        deleteCookie.Should().Contain("expires=", Exactly.Once());
    }

    [Fact]
    public async Task Logout_WhenRefreshCookieIsMissing_ShouldReturnUnauthorized()
    {
        var response = await Client.PostAsync("/api/auth/logout", null, Ct);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<AuthContext> RegisterAsync()
    {
        var email = $"tenant_{Guid.NewGuid()}@test.com";
        var password = "Password123!";

        var response = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password, password, "Tenant"),
            Ct);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await ReadResponseAsync<AuthUserResponse>(response);
        authResponse.Should().NotBeNull();

        return new AuthContext(
            response,
            authResponse!,
            GetRefreshCookie(response),
            email,
            password);
    }

    private static string GetRefreshCookie(HttpResponseMessage response)
    {
        var setCookie = response.Headers.GetValues("Set-Cookie")
            .Single(header => header.StartsWith("RefreshToken=", StringComparison.OrdinalIgnoreCase));

        return setCookie.Split(';', 2)[0];
    }

    private async Task<HttpResponseMessage> SendAuthRequestAsync(
        HttpMethod method,
        string url,
        string refreshCookie)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Cookie", refreshCookie);

        return await Client.SendAsync(request, Ct);
    }

    private new sealed record AuthContext(
        HttpResponseMessage Response,
        AuthUserResponse AuthResponse,
        string RefreshCookie,
        string Email,
        string Password);
}
