using api.Data;
using api.DTOs;
using api.ExceptionHandlers;
using api.Mappers;
using api.Models;
using api.Models.DTOs.Domain;
using Microsoft.EntityFrameworkCore;

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

    public async Task<MediaDto> UpdateMedia(MediaDto updatedMedia, int id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var mediaToUpdate = await context
                .Medias.Include(m => m.Genres)
                .Include(m => m.Episodes)
                .Include(m => m.MediaPersonRoles)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mediaToUpdate is null)
            {
                throw new NotFoundException("Media not found");
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

            // Update episodes
            mediaToUpdate.Episodes.Clear();

            List<Episode> newEpisodes = [];
            if (updatedMedia.Episodes != null)
            {
                newEpisodes = await context
                    .Episodes.Where(e => updatedMedia.Episodes.Contains(e.Id))
                    .ToListAsync();
            }

            foreach (var newEpisode in newEpisodes)
            {
                mediaToUpdate.Episodes.Add(newEpisode);
            }

            // Update MediaPersonRoles
            // Clear existing MediaPersonRoles for this media
            var existingMediaPersonRoles = await context
                .MediaPersonRoles.Where(mpr => mpr.MediaId == id)
                .ToListAsync();

            context.MediaPersonRoles.RemoveRange(existingMediaPersonRoles);

            // Get all role names from the credits
            var roleNames = updatedMedia.Credits.SelectMany(c => c.Roles).Distinct().ToList();
            var personIds = updatedMedia.Credits.Select(c => c.PersonId).ToList();

            // Fetch existing roles and persons from database
            var existingRoles = await context
                .Roles.Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            var existingPersons = await context
                .Persons.Where(p => personIds.Contains(p.Id))
                .ToListAsync();

            // Create new MediaPersonRole entities
            foreach (var credit in updatedMedia.Credits)
            {
                var person = existingPersons.FirstOrDefault(p => p.Id == credit.PersonId);

                if (person == null)
                {
                    continue;
                }

                foreach (var roleName in credit.Roles)
                {
                    var role = existingRoles.FirstOrDefault(r => r.Name == roleName);

                    if (role == null)
                    {
                        continue;
                    }

                    var mediaPersonRole = new MediaPersonRole
                    {
                        MediaId = id,
                        PersonId = credit.PersonId,
                        RoleId = role.Id,
                    };

                    mediaToUpdate.MediaPersonRoles.Add(mediaPersonRole);
                }
            }

            mediaToUpdate.Name = updatedMedia.Name;
            mediaToUpdate.Type = updatedMedia.Type;
            mediaToUpdate.Runtime = updatedMedia.Runtime;
            mediaToUpdate.Description = updatedMedia.Description;
            mediaToUpdate.Cover = updatedMedia.Cover;
            mediaToUpdate.AgeLimit = updatedMedia.AgeLimit;
            mediaToUpdate.Release = updatedMedia.Release;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return mediaToUpdate.FromSqlEntityToDto();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "An error occurred while trying to update the media");
            throw;
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
        await context.Medias.Where(m => m.Id == id).ExecuteDeleteAsync();
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
