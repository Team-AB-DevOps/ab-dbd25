using api.Models.Sql;

namespace api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User> CreateUser(User user);
    Task<User?> GetById(int id);
    Task SaveChangesAsync();
}
