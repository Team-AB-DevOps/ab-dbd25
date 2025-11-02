using api.Data;
using api.DTOs;
using api.ExceptionHandlers;
using api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class SqlRepository(DataContext context) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        var medias = await context.Medias.ToListAsync();

        return medias.Select(media => media.FromSqlEntityToDto()).ToList();
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        var users = await context.Users.ToListAsync();
        
        return users.Select(user => user.FromSqlEntityToDto()).ToList();
    }

    public async Task<UserDto> GetUserById(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User with ID " + id + " not found.");
        }
        
        return user.FromSqlEntityToDto();
    }
}
