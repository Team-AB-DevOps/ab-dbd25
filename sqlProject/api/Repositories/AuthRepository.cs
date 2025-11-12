using api.Data;
using api.ExceptionHandlers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class AuthRepository : IUserRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(DataContext dataContext, ILogger<AuthRepository> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dataContext
            .Users.Include(u => u.Privileges)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUser(User user)
    {
        await using var transaction = await _dataContext.Database.BeginTransactionAsync();
        try
        {
            var createdUser = await _dataContext.Users.AddAsync(user);
            var profile = new Profile
            {
                UserId = createdUser.Entity.Id,
                Name = "Default",
                IsChild = false
            };
            var createdProfile = await _dataContext.Profiles.AddAsync(profile);
            var watchList = new WatchList
            {
                ProfileId = createdProfile.Entity.Id,
                IsLocked = false
            };
            await _dataContext.WatchLists.AddAsync(watchList);

            await SaveChangesAsync();
            await transaction.CommitAsync();
            return createdUser.Entity;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new BadRequestException("An error occurred while trying to create the user", e);
        }
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