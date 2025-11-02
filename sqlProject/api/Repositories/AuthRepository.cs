using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class AuthRepository : IUserRepository
{
    private readonly DataContext _dataContext;

    public AuthRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dataContext
            .Users.Include(u => u.Privileges)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUser(User user)
    {
        var createdUser = await _dataContext.Users.AddAsync(user);
        await SaveChangesAsync();
        return createdUser.Entity;
    }

    public async Task<User?> GetById(int id)
    {
        return await _dataContext
            .Users.Include(u => u.Privileges)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _dataContext.SaveChangesAsync();
    }
}
