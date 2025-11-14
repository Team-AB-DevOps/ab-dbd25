using api.Models.Sql;

namespace api.Services.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}
