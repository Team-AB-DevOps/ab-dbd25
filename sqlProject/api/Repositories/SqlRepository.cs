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
        var mediaList = await context
            .Medias.Include(x => x.Genres)
            .Include(x => x.Episodes)
            .Include(x => x.MediaPersonRoles)
            .ThenInclude(x => x.Role)
            .AsNoTracking()
            .ToListAsync();

        var dtos = mediaList.Select(media => media.FromSqlEntityToDto()).ToList();

        return dtos;
    }

    public async Task<MediaDto> GetMediaById(int id)
    {
        var media = await context
            .Medias.Where(m => m.Id == id)
            .Include(x => x.Genres)
            .Include(x => x.Episodes)
            .Include(x => x.MediaPersonRoles)
            .ThenInclude(x => x.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (media == null)
        {
            throw new BadRequestException("Media not found");
        }

        var dto = media.FromSqlEntityToDto();

        return dto;
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        var episodes = await context.Episodes.Where(x => x.MediaId == id).ToListAsync();

        if (episodes.Count == 0)
        {
            throw new BadRequestException($"No episodes found for media with ID: {id}");
        }

        var dto = episodes.Select(e => e.FromSqlEntityToDto()).ToList();

        return dto;
    }

    public async Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId)
    {
        var episode = await context
            .Episodes.Where(x => x.MediaId == id && x.Id == episodeId)
            .FirstOrDefaultAsync();

        if (episode is null)
        {
            throw new BadRequestException(
                $"No episode found with ID: {episodeId}, for media with ID: {id}"
            );
        }

        var dto = episode.FromSqlEntityToDto();

        return dto;
    }
}
