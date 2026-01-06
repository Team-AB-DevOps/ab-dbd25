using api.Data;
using api.ExceptionHandlers;
using api.Models;
using api.Models.Sql;
using api.Repositories.Interfaces;
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
            // First, save the user to get the generated ID
            var createdUser = await _dataContext.Users.AddAsync(user);
            await SaveChangesAsync();
            
            // Now create the profile with the actual user ID
            var profile = new Profile
            {
                UserId = createdUser.Entity.Id,
                Name = "Default",
                IsChild = false,
            };
            
            // Create the watchlist with the profile navigation property
            var watchList = new WatchList
            {
                Profile = profile,
                IsLocked = false,
            };
            
            await _dataContext.Profiles.AddAsync(profile);
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
