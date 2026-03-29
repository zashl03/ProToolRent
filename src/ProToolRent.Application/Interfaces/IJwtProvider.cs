using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
