using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SqlToGraph.Services;

public interface IPostgreSqlDataService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<Profile>> GetProfilesAsync();
    Task<IEnumerable<WatchList>> GetWatchListsAsync();
    Task<IEnumerable<Media>> GetMediasAsync();
    Task<IEnumerable<Episode>> GetEpisodesAsync();
    Task<IEnumerable<Person>> GetPersonsAsync();
    Task<IEnumerable<Genre>> GetGenresAsync();
    Task<IEnumerable<Role>> GetRolesAsync();
    Task<IEnumerable<Subscription>> GetSubscriptionsAsync();
    Task<IEnumerable<Privilege>> GetPrivilegesAsync();

    // Relationship queries
    Task<IEnumerable<(int UserId, int ProfileId)>> GetUserProfileRelationshipsAsync();
    Task<IEnumerable<(int ProfileId, int WatchListId)>> GetProfileWatchListsAsync();
    Task<IEnumerable<(int WatchListId, int MediaId)>> GetWatchListMediasAsync();
    Task<IEnumerable<(int MediaId, int EpisodeId)>> GetMediaEpisodesAsync();
    Task<IEnumerable<(int MediaId, int GenreId)>> GetMediaGenresAsync();
    Task<IEnumerable<(int PersonId, int MediaId, string Role)>> GetPersonMediaRolesAsync();
    Task<
        IEnumerable<(
            int ProfileId,
            int MediaId,
            int Rating,
            string ReviewText,
            DateTime ReviewDate
        )>
    > GetReviewsAsync();
    Task<IEnumerable<(int UserId, int SubscriptionId)>> GetUserSubscriptionsAsync();
    Task<IEnumerable<(int SubscriptionId, int GenreId)>> GetSubscriptionGenresAsync();
    Task<IEnumerable<(int UserId, int PrivilegeId)>> GetUserPrivilegesAsync();

    Task<bool> TestConnectionAsync();
}

public class PostgreSqlDataService : IPostgreSqlDataService
{
    private readonly DataContext _context;
    private readonly ILogger<PostgreSqlDataService> _logger;

    public PostgreSqlDataService(
        IConfiguration configuration,
        ILogger<PostgreSqlDataService> logger
    )
    {
        _logger = logger;

        var connectionString = configuration.GetConnectionString("PostgreSql");
        logger.LogInformation(connectionString);
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new DataContext(options);

        _logger.LogInformation("Connected to PostgreSQL database using Entity Framework");
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _context.Database.CanConnectAsync();
            _logger.LogInformation("PostgreSQL connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostgreSQL connection test failed");
            return false;
        }
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        _logger.LogInformation("Fetching users from PostgreSQL...");
        var users = await _context.Users.ToListAsync();
        _logger.LogInformation($"Retrieved {users.Count} users");
        return users;
    }

    public async Task<IEnumerable<Profile>> GetProfilesAsync()
    {
        _logger.LogInformation("Fetching profiles from PostgreSQL...");
        var profiles = await _context.Profiles.ToListAsync();
        _logger.LogInformation($"Retrieved {profiles.Count} profiles");
        return profiles;
    }

    public async Task<IEnumerable<WatchList>> GetWatchListsAsync()
    {
        _logger.LogInformation("Fetching watch lists from PostgreSQL...");
        var watchLists = await _context.WatchLists.ToListAsync();
        _logger.LogInformation($"Retrieved {watchLists.Count} watch lists");
        return watchLists;
    }

    public async Task<IEnumerable<Media>> GetMediasAsync()
    {
        _logger.LogInformation("Fetching media from PostgreSQL...");
        var medias = await _context.Medias.ToListAsync();
        _logger.LogInformation($"Retrieved {medias.Count} media items");
        return medias;
    }

    public async Task<IEnumerable<Episode>> GetEpisodesAsync()
    {
        _logger.LogInformation("Fetching episodes from PostgreSQL...");
        var episodes = await _context.Episodes.ToListAsync();
        _logger.LogInformation($"Retrieved {episodes.Count} episodes");
        return episodes;
    }

    public async Task<IEnumerable<Person>> GetPersonsAsync()
    {
        _logger.LogInformation("Fetching persons from PostgreSQL...");
        var persons = await _context.Persons.ToListAsync();
        _logger.LogInformation($"Retrieved {persons.Count} persons");
        return persons;
    }

    public async Task<IEnumerable<Genre>> GetGenresAsync()
    {
        _logger.LogInformation("Fetching genres from PostgreSQL...");
        var genres = await _context.Genres.ToListAsync();
        _logger.LogInformation($"Retrieved {genres.Count} genres");
        return genres;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        _logger.LogInformation("Fetching roles from PostgreSQL...");
        var roles = await _context.Roles.ToListAsync();
        _logger.LogInformation($"Retrieved {roles.Count} roles");
        return roles;
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync()
    {
        _logger.LogInformation("Fetching subscriptions from PostgreSQL...");
        var subscriptions = await _context.Subscriptions.ToListAsync();
        _logger.LogInformation($"Retrieved {subscriptions.Count} subscriptions");
        return subscriptions;
    }

    public async Task<IEnumerable<Privilege>> GetPrivilegesAsync()
    {
        _logger.LogInformation("Fetching privileges from PostgreSQL...");
        var privileges = await _context.Privileges.ToListAsync();
        _logger.LogInformation($"Retrieved {privileges.Count} privileges");
        return privileges;
    }

    // Relationship queries
    public async Task<IEnumerable<(int UserId, int ProfileId)>> GetUserProfileRelationshipsAsync()
    {
        _logger.LogInformation("Fetching user-profile relationships...");
        var relationships = await _context
            .Profiles.Select(p => new { p.UserId, p.Id })
            .ToListAsync();

        _logger.LogInformation($"Retrieved {relationships.Count} user-profile relationships");
        return relationships.Select(r => (r.UserId, r.Id));
    }

    public async Task<IEnumerable<(int ProfileId, int WatchListId)>> GetProfileWatchListsAsync()
    {
        _logger.LogInformation("Fetching profile-watchlist relationships...");
        var relationships = await _context
            .WatchLists.Select(w => new { ProfileId = w.ProfileId, WatchListId = w.ProfileId }) // WatchList uses ProfileId as both FK and PK
            .ToListAsync();

        _logger.LogInformation($"Retrieved {relationships.Count} profile-watchlist relationships");
        return relationships.Select(r => (r.ProfileId, r.WatchListId));
    }

    public async Task<IEnumerable<(int WatchListId, int MediaId)>> GetWatchListMediasAsync()
    {
        _logger.LogInformation("Fetching watchlist-media relationships...");

        try
        {
            var relationships = await _context
                .WatchLists.Include(w => w.Medias)
                .SelectMany(w =>
                    w.Medias.Select(m => new { WatchListId = w.ProfileId, MediaId = m.Id })
                )
                .ToListAsync();

            _logger.LogInformation(
                $"Retrieved {relationships.Count} watchlist-media relationships"
            );
            return relationships.Select(r => (r.WatchListId, r.MediaId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch watchlist-media relationships, returning empty collection"
            );
            return new List<(int, int)>();
        }
    }

    public async Task<IEnumerable<(int MediaId, int EpisodeId)>> GetMediaEpisodesAsync()
    {
        _logger.LogInformation("Fetching media-episode relationships...");
        var relationships = await _context
            .Episodes.Select(e => new { e.MediaId, e.Id })
            .ToListAsync();

        _logger.LogInformation($"Retrieved {relationships.Count} media-episode relationships");
        return relationships.Select(r => (r.MediaId, r.Id));
    }

    public async Task<IEnumerable<(int MediaId, int GenreId)>> GetMediaGenresAsync()
    {
        _logger.LogInformation("Fetching media-genre relationships...");

        try
        {
            var relationships = await _context
                .Medias.Include(m => m.Genres)
                .SelectMany(m => m.Genres.Select(g => new { MediaId = m.Id, GenreId = g.Id }))
                .ToListAsync();

            _logger.LogInformation($"Retrieved {relationships.Count} media-genre relationships");
            return relationships.Select(r => (r.MediaId, r.GenreId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch media-genre relationships, returning empty collection"
            );
            return new List<(int, int)>();
        }
    }

    public async Task<
        IEnumerable<(int PersonId, int MediaId, string Role)>
    > GetPersonMediaRolesAsync()
    {
        _logger.LogInformation("Fetching person-media-role relationships...");

        try
        {
            var relationships = await _context
                .MediaPersonRoles.Include(mpr => mpr.Person)
                .Include(mpr => mpr.Media)
                .Include(mpr => mpr.Role)
                .Select(mpr => new
                {
                    mpr.PersonId,
                    mpr.MediaId,
                    RoleName = mpr.Role.Name,
                })
                .ToListAsync();

            _logger.LogInformation(
                $"Retrieved {relationships.Count} person-media-role relationships"
            );
            return relationships.Select(r => (r.PersonId, r.MediaId, r.RoleName));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch person-media-role relationships, returning empty collection"
            );
            return new List<(int, int, string)>();
        }
    }

    public async Task<
        IEnumerable<(
            int ProfileId,
            int MediaId,
            int Rating,
            string ReviewText,
            DateTime ReviewDate
        )>
    > GetReviewsAsync()
    {
        _logger.LogInformation("Fetching profile review relationships...");

        try
        {
            var relationships = await _context
                .Reviews.Select(r => new
                {
                    r.ProfileId,
                    r.MediaId,
                    r.Rating,
                    Description = r.Description,
                    r.CreatedAt,
                })
                .ToListAsync();

            _logger.LogInformation($"Retrieved {relationships.Count} profile review relationships");
            return relationships.Select(r =>
                (r.ProfileId, r.MediaId, r.Rating, r.Description ?? "", r.CreatedAt)
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch profile review relationships, returning empty collection"
            );
            return new List<(int, int, int, string, DateTime)>();
        }
    }

    public async Task<IEnumerable<(int UserId, int SubscriptionId)>> GetUserSubscriptionsAsync()
    {
        _logger.LogInformation("Fetching user-subscription relationships...");

        try
        {
            var relationships = await _context
                .Users.Include(u => u.Subscriptions)
                .SelectMany(u =>
                    u.Subscriptions.Select(s => new { UserId = u.Id, SubscriptionId = s.Id })
                )
                .ToListAsync();

            _logger.LogInformation(
                $"Retrieved {relationships.Count} user-subscription relationships"
            );
            return relationships.Select(r => (r.UserId, r.SubscriptionId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch user-subscription relationships, returning empty collection"
            );
            return new List<(int, int)>();
        }
    }

    public async Task<IEnumerable<(int SubscriptionId, int GenreId)>> GetSubscriptionGenresAsync()
    {
        _logger.LogInformation("Fetching subscription-genre relationships...");

        // This relationship might not exist in the current schema
        _logger.LogWarning("Subscription-Genre relationship not found in current schema");
        return new List<(int, int)>();
    }

    public async Task<IEnumerable<(int UserId, int PrivilegeId)>> GetUserPrivilegesAsync()
    {
        _logger.LogInformation("Fetching user-privilege relationships...");

        try
        {
            var relationships = await _context
                .Users.Include(u => u.Privileges)
                .SelectMany(u =>
                    u.Privileges.Select(p => new { UserId = u.Id, PrivilegeId = p.Id })
                )
                .ToListAsync();

            _logger.LogInformation($"Retrieved {relationships.Count} user-privilege relationships");
            return relationships.Select(r => (r.UserId, r.PrivilegeId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not fetch user-privilege relationships, returning empty collection"
            );
            return new List<(int, int)>();
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
