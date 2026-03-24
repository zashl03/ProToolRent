
namespace ProToolRent.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public string SecretKey { get; init; } = string.Empty;
        public int ExpiryHours { get; init; }
    }
}
