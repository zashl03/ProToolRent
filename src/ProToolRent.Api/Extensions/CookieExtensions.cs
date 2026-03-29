namespace ProToolRent.Api.Extensions;

public static class CookieExtensions
{
    public static void AppendRefreshToken(this HttpResponse response, string refreshToken)
    {
        response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    public static void DeleteRefreshToken(this HttpResponse response)
    {
        response.Cookies.Delete("RefreshToken");
    }

    public static string? GetRefreshToken(this HttpRequest request)
    {
        return request.Cookies["RefreshToken"];
    }
}
