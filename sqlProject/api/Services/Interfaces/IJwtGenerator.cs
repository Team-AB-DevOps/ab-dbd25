using api.Models;

namespace api.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}