using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using api.Models;
using MigrationTool.Models;

namespace MigrationTool.Services;

public class MigrationService
{
    private readonly PostgresService _postgresService;
    private readonly MongoService _mongoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MigrationService> _logger;
    private readonly int _batchSize;

    public MigrationService(
        PostgresService postgresService, 
        MongoService mongoService,
        IConfiguration configuration,
        ILogger<MigrationService> logger)
    {
        _postgresService = postgresService;
        _mongoService = mongoService;
        _configuration = configuration;
        _logger = logger;
        _batchSize = configuration.GetValue<int>("Migration:BatchSize", 1000);
    }

    public async Task<bool> TestConnectionsAsync()
    {
        _logger.LogInformation("Testing database connections...");
        
        var postgresConnected = await _postgresService.TestConnectionAsync();
        var mongoConnected = await _mongoService.TestConnectionAsync();

        if (postgresConnected && mongoConnected)
        {
            _logger.LogInformation("All database connections successful");
            return true;
        }

        _logger.LogError("Database connection tests failed");
        return false;
    }

    public async Task RunFullMigrationAsync()
    {
        _logger.LogInformation("Starting full migration from PostgreSQL to MongoDB with embedded documents...");

        try
        {
            // Test connections first
            if (!await TestConnectionsAsync())
            {
                throw new Exception("Database connection tests failed");
            }

            // Create MongoDB indexes
            await _mongoService.CreateIndexesAsync();

            // Migrate collections in order
            await MigrateSubscriptionsAsync();
            await MigrateAccountsAsync();
            await MigrateUsersWithEmbeddedDataAsync();
            await MigrateMediaWithEmbeddedDataAsync();
            await MigratePersonsAsync();
            await MigrateEpisodesAsync();

            _logger.LogInformation("Full migration completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed");
            throw;
        }
    }

    private async Task MigrateSubscriptionsAsync()
    {
        _logger.LogInformation("Migrating Subscriptions...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoSubscription>("subscriptions");

        var subscriptions = await context.Subscriptions.ToListAsync();
        
        var mongoSubscriptions = subscriptions.Select(s => new MongoSubscription
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price
        }).ToList();

        if (mongoSubscriptions.Count > 0)
        {
            await mongoCollection.InsertManyAsync(mongoSubscriptions);
            _logger.LogInformation($"Migrated {mongoSubscriptions.Count} subscriptions");
        }

        _logger.LogInformation("Subscriptions migration completed");
    }

    private async Task MigrateAccountsAsync()
    {
        _logger.LogInformation("Migrating Accounts (from Users table)...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoAccount>("accounts");

        var users = await context.Users.ToListAsync();
        
        var accounts = users.Select(u => new MongoAccount
        {
            Id = u.Id, // Using same ID for account
            Email = u.Email,
            Password = u.Password,
            UserId = u.Id
        }).ToList();

        if (accounts.Count > 0)
        {
            await mongoCollection.InsertManyAsync(accounts);
            _logger.LogInformation($"Migrated {accounts.Count} accounts");
        }

        _logger.LogInformation("Accounts migration completed");
    }

    private async Task MigrateUsersWithEmbeddedDataAsync()
    {
        _logger.LogInformation("Migrating Users with embedded profiles, subscriptions, and privileges...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoUser>("users");

        // Get all users with related data
        var users = await context.Users
            .Include(u => u.Privileges)
            .Include(u => u.Subscriptions)
            .Include(u => u.Profiles)
            .ToListAsync();

        var mongoUsers = new List<MongoUser>();

        foreach (var user in users)
        {
            // Get profiles with embedded watchlists and reviews
            var profiles = await context.Profiles
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var mongoProfiles = new List<MongoProfile>();
            
            foreach (var profile in profiles)
            {
                // Get watchlist for this profile
                var watchlist = await context.WatchLists
                    .Include(w => w.Medias)
                    .FirstOrDefaultAsync(w => w.ProfileId == profile.Id);

                // Get reviews for this profile
                var reviews = await context.Reviews
                    .Where(r => r.ProfileId == profile.Id)
                    .ToListAsync();

                var mongoProfile = new MongoProfile
                {
                    Name = profile.Name,
                    IsChild = profile.IsChild ?? false,
                    Watchlist = new MongoWatchlist
                    {
                        IsLocked = watchlist?.IsLocked ?? false,
                        Medias = watchlist?.Medias?.Select(m => m.Id).ToList() ?? new List<int>()
                    },
                    Reviews = reviews.Select((r, index) => new MongoReview
                    {
                        Id = index + 1, // Generate sequential ID since Review doesn't have an Id field
                        MediaId = r.MediaId,
                        Rating = r.Rating,
                        Description = r.Description ?? ""
                    }).ToList()
                };

                mongoProfiles.Add(mongoProfile);
            }

            var mongoUser = new MongoUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Subscriptions = user.Subscriptions?.Select(s => s.Id).ToList() ?? new List<int>(),
                Privileges = user.Privileges?.Select(p => p.Name).ToList() ?? new List<string>(),
                Profiles = mongoProfiles
            };

            mongoUsers.Add(mongoUser);
        }

        if (mongoUsers.Count > 0)
        {
            await mongoCollection.InsertManyAsync(mongoUsers);
            _logger.LogInformation($"Migrated {mongoUsers.Count} users with embedded data");
        }

        _logger.LogInformation("Users migration completed");
    }

    private async Task MigrateMediaWithEmbeddedDataAsync()
    {
        _logger.LogInformation("Migrating Media with embedded genres, episodes, and credits...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoMedia>("medias");

        var medias = await context.Medias
            .Include(m => m.Genres)
            .Include(m => m.Episodes)
            .Include(m => m.MediaPersonRoles)
                .ThenInclude(mpr => mpr.Role)
            .ToListAsync();

        var mongoMedias = new List<MongoMedia>();

        foreach (var media in medias)
        {
            // Group credits by person and combine their roles
            var credits = media.MediaPersonRoles
                .GroupBy(mpr => mpr.PersonId)
                .Select(g => new MongoCredit
                {
                    PersonId = g.Key,
                    Roles = g.Select(mpr => mpr.Role.Name).ToList()
                }).ToList();

            var mongoMedia = new MongoMedia
            {
                Id = media.Id,
                Name = media.Name,
                Type = media.Type,
                Runtime = media.Runtime,
                Description = media.Description,
                Cover = media.Cover,
                AgeLimit = media.AgeLimit ?? 0,
                Release = media.Release.ToString("yyyy-MM-dd"),
                Genres = media.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
                Episodes = media.Episodes?.Select(e => e.Id).ToList() ?? new List<int>(),
                Credits = credits
            };

            mongoMedias.Add(mongoMedia);
        }

        if (mongoMedias.Count > 0)
        {
            await mongoCollection.InsertManyAsync(mongoMedias);
            _logger.LogInformation($"Migrated {mongoMedias.Count} media items with embedded data");
        }

        _logger.LogInformation("Media migration completed");
    }

    private async Task MigratePersonsAsync()
    {
        _logger.LogInformation("Migrating Persons...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoPerson>("persons");

        var persons = await context.Persons.ToListAsync();
        
        var mongoPersons = persons.Select(p => new MongoPerson
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Gender = p.Gender,
            BirthDate = p.BirthDate.ToString("yyyy-MM-dd")
        }).ToList();

        if (mongoPersons.Count > 0)
        {
            await mongoCollection.InsertManyAsync(mongoPersons);
            _logger.LogInformation($"Migrated {mongoPersons.Count} persons");
        }

        _logger.LogInformation("Persons migration completed");
    }

    private async Task MigrateEpisodesAsync()
    {
        _logger.LogInformation("Migrating Episodes...");
        
        var context = _postgresService.GetContext();
        var mongoCollection = _mongoService.GetCollection<MongoEpisode>("episodes");

        var episodes = await context.Episodes.ToListAsync();
        
        var mongoEpisodes = episodes.Select(e => new MongoEpisode
        {
            Id = e.Id,
            Name = e.Name,
            SeasonCount = e.SeasonCount ?? 1,
            EpisodeCount = e.EpisodeCount,
            Runtime = e.Runtime,
            Description = e.Description,
            Release = e.Release.ToString("yyyy-MM-dd")
        }).ToList();

        if (mongoEpisodes.Count > 0)
        {
            await mongoCollection.InsertManyAsync(mongoEpisodes);
            _logger.LogInformation($"Migrated {mongoEpisodes.Count} episodes");
        }

        _logger.LogInformation("Episodes migration completed");
    }


}
