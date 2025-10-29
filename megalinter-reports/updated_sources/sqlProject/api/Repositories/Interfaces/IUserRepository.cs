using api.Models;

namespace api.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User> CreateUser(User user);
    Task<User?> GetById(int id);
    Task SaveChangesAsync();
}
