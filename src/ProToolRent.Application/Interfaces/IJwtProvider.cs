using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}
