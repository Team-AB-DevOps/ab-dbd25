using api.Data;
using api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class SqlRepository(DataContext context) : IRepository
{
    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var medias = await context.Medias.ToListAsync();

        return medias.Select(media => new MediaDTO(
            media.Id,
            media.Name,
            media.Type,
            media.Runtime,
            media.Description,
            media.Cover,
            media.AgeLimit,
            media.Release
        )).ToList();
    }
}