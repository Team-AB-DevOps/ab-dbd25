using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class SqlRepository(DataContext context) : IRepository
{
    public async Task<List<Media>> GetAllMedias()
    {
        return await context.Medias.ToListAsync();
    }
}