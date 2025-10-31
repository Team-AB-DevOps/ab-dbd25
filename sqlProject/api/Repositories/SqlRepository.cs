using api.Data;
using api.DTOs;
using api.ExceptionHandlers;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class SqlRepository(DataContext context) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        var mediaList = await context.Medias
            .Include(x => x.Genres)
            .Include(x => x.Episodes)
            .Include(x => x.MediaPersonRoles)
            .ThenInclude(x => x.Role)
            .AsNoTracking()
            .ToListAsync();

        var dtos = mediaList.Select(media => new MediaDto(
            media.Id,
            media.Name,
            media.Type,
            media.Runtime,
            media.Description,
            media.Cover,
            media.AgeLimit,
            media.Release,
            media.Genres.Select(g => g.Name).ToArray(),
            media.Episodes?.Count > 0 ? media.Episodes.Select(e => e.Id).ToArray() : null,
            media.MediaPersonRoles
                .GroupBy(x => x.PersonId)
                .Select(group => new MediaCreditsDto(
                    group.Key,
                    group.Select(x => x.Role.Name).ToArray()
                ))
                .ToArray()
        )).ToList();

        return dtos;
    }

    public async Task<MediaDto> GetMediaById(int id)
    {
        var media = await context.Medias.Where(m => m.Id == id)
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

        var dto = new MediaDto(
            media.Id,
            media.Name,
            media.Type,
            media.Runtime,
            media.Description,
            media.Cover,
            media.AgeLimit,
            media.Release,
            media.Genres.Select(g => g.Name).ToArray(),
            media.Episodes?.Count > 0 ? media.Episodes.Select(e => e.Id).ToArray() : null,
            media.MediaPersonRoles
                .GroupBy(x => x.PersonId)
                .Select(group => new MediaCreditsDto(
                    group.Key,
                    group.Select(x => x.Role.Name).ToArray()
                ))
                .ToArray()
        );

        return dto;
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        var episodes = await context.Episodes.Where(x => x.MediaId == id).ToListAsync();

        if (episodes.Count == 0)
        {
            throw new BadRequestException($"No episodes found for media with ID: {id}");
        }

        var dto = episodes.Select(e => new EpisodeDto(
                e.Id,
                e.Name,
                e.SeasonCount ?? 1,
                e.EpisodeCount,
                e.Runtime,
                e.Description,
                e.Release
            )
        ).ToList();

        return dto;
    }

    public async Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId)
    {
        var episode = await context.Episodes.Where(x => x.MediaId == id && x.Id == episodeId).FirstOrDefaultAsync();

        if (episode is null)
        {
            throw new BadRequestException($"No episode found with ID: {episodeId}, for media with ID: {id}");
        }

        var dto = new EpisodeDto(
            episode.Id,
            episode.Name,
            episode.SeasonCount ?? 1,
            episode.EpisodeCount,
            episode.Runtime,
            episode.Description,
            episode.Release
        );

        return dto;
    }
}