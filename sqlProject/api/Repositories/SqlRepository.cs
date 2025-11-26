using api.Data;
using api.ExceptionHandlers;
using api.Mappers;
using api.Models.DTOs.Domain;
using api.Models.Sql;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api.Repositories;

public class SqlRepository(DataContext context, ILogger<SqlRepository> logger) : IRepository
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
            throw new NotFoundException("Media not found");
        }

        var dto = media.FromSqlEntityToDto();

        return dto;
    }

    public async Task<MediaDto> UpdateMedia(UpdateMediaDto updatedMedia, int id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var mediaToUpdate = await context
                .Medias.Include(m => m.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mediaToUpdate is null)
            {
                throw new NotFoundException("Media not found");
            }

            if (mediaToUpdate.Id != updatedMedia.Id)
            {
                throw new BadRequestException(
                    $"URL ID ({id}) does not match the ID in the request body ({updatedMedia.Id})"
                );
            }

            // Update genres
            mediaToUpdate.Genres.Clear();

            var newGenres = await context
                .Genres.Where(g => updatedMedia.Genres.Contains(g.Name))
                .ToListAsync();

            foreach (var newGenre in newGenres)
            {
                mediaToUpdate.Genres.Add(newGenre);
            }

            mediaToUpdate.Name = updatedMedia.Name;
            mediaToUpdate.Type = updatedMedia.Type;
            mediaToUpdate.Runtime = updatedMedia.Runtime;
            mediaToUpdate.Description = updatedMedia.Description;
            mediaToUpdate.Cover = updatedMedia.Cover;
            mediaToUpdate.AgeLimit = updatedMedia.AgeLimit;
            mediaToUpdate.Release = updatedMedia.Release;

            await context.SaveChangesAsync();

            // Load Episodes and MediaPersonRoles after saving changes
            await context.Entry(mediaToUpdate).Collection(m => m.Episodes).LoadAsync();
            await context.Entry(mediaToUpdate).Collection(m => m.MediaPersonRoles).LoadAsync();
            await context
                .Entry(mediaToUpdate)
                .Collection(m => m.MediaPersonRoles)
                .Query()
                .Include(mpr => mpr.Role)
                .LoadAsync();

            await transaction.CommitAsync();

            return mediaToUpdate.FromSqlEntityToDto();
        }
        catch (DbUpdateException e)
        {
            await transaction.RollbackAsync();
            throw new BadRequestException("An error occurred while trying to update the media", e);
        }
    }

    public async Task<MediaDto> CreateMedia(CreateMediaDto newMedia)
    {
        var media = new Media
        {
            Name = newMedia.Name,
            Type = newMedia.Type,
            Runtime = newMedia.Runtime,
            Description = newMedia.Description,
            Cover = newMedia.Cover,
            AgeLimit = newMedia.AgeLimit,
            Release = newMedia.Release,
            CreatedAt = DateTime.UtcNow,
        };

        // Add genres
        if (newMedia.Genres.Length > 0)
        {
            var genres = await context
                .Genres.Where(g => newMedia.Genres.Contains(g.Name))
                .ToListAsync();

            foreach (var genre in genres)
            {
                media.Genres.Add(genre);
            }
        }

        context.Medias.Add(media);
        await context.SaveChangesAsync();

        return media.FromSqlEntityToDto();
    }

    public async Task DeleteMediaById(int id)
    {
        var result = await context.Medias.Where(m => m.Id == id).ExecuteDeleteAsync();

        if (result == 0)
        {
            throw new NotFoundException("Media not found");
        }
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        var episodes = await context.Episodes.Where(x => x.MediaId == id).ToListAsync();

        if (episodes.Count == 0)
        {
            throw new NotFoundException($"No episodes found for media with ID: {id}");
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
            throw new NotFoundException(
                $"No episode found with ID: {episodeId}, for media with ID: {id}"
            );
        }

        var dto = episode.FromSqlEntityToDto();

        return dto;
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        var users = await GetUsersWithIncludes().ToListAsync();
        return users.Select(user => user.FromSqlEntityToDto()).ToList();
    }

    public async Task<UserDto> GetUserById(int id)
    {
        var user = await GetUsersWithIncludes().FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            throw new NotFoundException("User with ID " + id + " not found.");
        }

        return user.FromSqlEntityToDto();
    }

    public async Task AddMediaToWatchList(int userId, int profileId, int mediaId)
    {
        try
        {
            // Call the stored procedure
            await context.Database.ExecuteSqlRawAsync(
                "CALL add_to_watchlist({0}, {1}, {2})",
                userId,
                profileId,
                mediaId
            );
        }
        catch (PostgresException ex)
        {
            throw new BadRequestException(ex.InnerException?.Message ?? ex.Message);
        }
    }

    // Private helper to encapsulate common include logic for users
    private IQueryable<User> GetUsersWithIncludes()
    {
        return context
            .Users.Include(u => u.Privileges)
            .Include(u => u.Subscriptions)
            .Include(u => u.Profiles)
            .ThenInclude(p => p.WatchList)
            .ThenInclude(w => w.Medias)
            .Include(u => u.Profiles)
            .ThenInclude(p => p.Reviews);
    }
}
