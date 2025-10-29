using api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlToGraph.Models;
using SqlToGraph.Services;

namespace SqlToGraph.Services;

public interface IMigrationOrchestrator
{
    Task RunMigrationAsync();
    Task TestPostgreSqlConnectionAsync();
    Task TestNeo4jConnectionAsync();
}

public class MigrationOrchestrator : IMigrationOrchestrator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly INeo4jMigrationService _neo4jService;
    private readonly ILogger<MigrationOrchestrator> _logger;

    public MigrationOrchestrator(
        IServiceScopeFactory serviceScopeFactory,
        INeo4jMigrationService neo4jService,
        ILogger<MigrationOrchestrator> logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _neo4jService = neo4jService;
        _logger = logger;
    }

    public async Task RunMigrationAsync()
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting migration process...");

        try
        {
            // Step 1: Test connections
            _logger.LogInformation("Step 1: Testing database connections...");

            bool postgresConnected;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var postgreSqlService =
                    scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                postgresConnected = await postgreSqlService.TestConnectionAsync();
            }

            var neo4jConnected = await _neo4jService.TestConnectionAsync();

            if (!postgresConnected || !neo4jConnected)
            {
                throw new Exception("Database connection tests failed");
            }

            // Step 2: Initialize Neo4j database
            _logger.LogInformation("Step 2: Initializing Neo4j database...");
            await _neo4jService.InitializeDatabaseAsync();

            // Step 3: Extract data from PostgreSQL
            _logger.LogInformation("Step 3: Extracting data from PostgreSQL...");

            // Create separate scoped tasks to avoid DbContext concurrency issues
            var usersTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetUsersAsync();
            });

            var profilesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetProfilesAsync();
            });

            var watchListsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetWatchListsAsync();
            });

            var mediasTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetMediasAsync();
            });

            var episodesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetEpisodesAsync();
            });

            var personsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetPersonsAsync();
            });

            var genresTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetGenresAsync();
            });

            var rolesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetRolesAsync();
            });

            var subscriptionsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetSubscriptionsAsync();
            });

            var privilegesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetPrivilegesAsync();
            });

            await Task.WhenAll(
                usersTask,
                profilesTask,
                watchListsTask,
                mediasTask,
                episodesTask,
                personsTask,
                genresTask,
                rolesTask,
                subscriptionsTask,
                privilegesTask
            );

            var users = await usersTask;
            var profiles = await profilesTask;
            var watchLists = await watchListsTask;
            var medias = await mediasTask;
            var episodes = await episodesTask;
            var persons = await personsTask;
            var genres = await genresTask;
            var roles = await rolesTask;
            var subscriptions = await subscriptionsTask;
            var privileges = await privilegesTask;

            _logger.LogInformation(
                "Extracted {UserCount} users, {ProfileCount} profiles, {WatchListCount} watchlists, {MediaCount} media items",
                users.Count(),
                profiles.Count(),
                watchLists.Count(),
                medias.Count()
            );

            // Step 4: Transform API models to graph models
            _logger.LogInformation("Step 4: Transforming data models...");

            var graphUsers = users.Select(u => u.ToGraphUser()).ToList();
            var graphProfiles = profiles.Select(p => p.ToGraphProfile()).ToList();
            var graphWatchLists = watchLists.Select(w => w.ToGraphWatchList()).ToList();
            var graphMedias = medias.Select(m => m.ToGraphMedia()).ToList();
            var graphEpisodes = episodes.Select(e => e.ToGraphEpisode()).ToList();
            var graphPersons = persons.Select(p => p.ToGraphPerson()).ToList();
            var graphGenres = genres.Select(g => g.ToGraphGenre()).ToList();
            var graphRoles = roles.Select(r => r.ToGraphRole()).ToList();
            var graphSubscriptions = subscriptions.Select(s => s.ToGraphSubscription()).ToList();
            var graphPrivileges = privileges.Select(p => p.ToGraphPrivilege()).ToList();

            // Step 5: Migrate nodes to Neo4j
            _logger.LogInformation("Step 5: Migrating nodes to Neo4j...");
            await _neo4jService.MigrateNodesAsync(
                graphUsers,
                graphProfiles,
                graphWatchLists,
                graphMedias,
                graphEpisodes,
                graphPersons,
                graphGenres,
                graphRoles,
                graphSubscriptions,
                graphPrivileges
            );

            // Step 6: Extract relationship data
            _logger.LogInformation("Step 6: Extracting relationship data from PostgreSQL...");

            // Create separate scoped tasks to avoid DbContext concurrency issues
            var userProfilesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetUserProfileRelationshipsAsync();
            });

            var profileWatchListsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetProfileWatchListsAsync();
            });

            var watchListMediasTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetWatchListMediasAsync();
            });

            var mediaEpisodesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetMediaEpisodesAsync();
            });

            var mediaGenresTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetMediaGenresAsync();
            });

            var personMediaRolesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetPersonMediaRolesAsync();
            });

            var reviewsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetReviewsAsync();
            });

            var userSubscriptionsTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetUserSubscriptionsAsync();
            });

            var subscriptionGenresTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetSubscriptionGenresAsync();
            });

            var userPrivilegesTask = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
                return await service.GetUserPrivilegesAsync();
            });

            await Task.WhenAll(
                userProfilesTask,
                profileWatchListsTask,
                watchListMediasTask,
                mediaEpisodesTask,
                mediaGenresTask,
                personMediaRolesTask,
                reviewsTask,
                userSubscriptionsTask,
                subscriptionGenresTask,
                userPrivilegesTask
            );

            var userProfiles = await userProfilesTask;
            var profileWatchLists = await profileWatchListsTask;
            var watchListMedias = await watchListMediasTask;
            var mediaEpisodes = await mediaEpisodesTask;
            var mediaGenres = await mediaGenresTask;
            var personMediaRoles = await personMediaRolesTask;
            var reviews = await reviewsTask;
            var userSubscriptions = await userSubscriptionsTask;
            var subscriptionGenres = await subscriptionGenresTask;
            var userPrivileges = await userPrivilegesTask;

            _logger.LogInformation(
                "Extracted {UserProfileCount} user-profile, {ReviewCount} review, {PersonMediaRoleCount} person-media-role relationships",
                userProfiles.Count(),
                reviews.Count(),
                personMediaRoles.Count()
            );

            // Step 7: Migrate relationships to Neo4j
            _logger.LogInformation("Step 7: Migrating relationships to Neo4j...");
            await _neo4jService.MigrateRelationshipsAsync(
                userProfiles,
                profileWatchLists,
                watchListMedias,
                mediaEpisodes,
                mediaGenres,
                personMediaRoles,
                reviews,
                userSubscriptions,
                subscriptionGenres,
                userPrivileges
            );

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Log migration statistics
            _logger.LogInformation(
                "🎉 Migration completed successfully in {Duration:hh\\:mm\\:ss}",
                duration
            );
            _logger.LogInformation("📊 Migration Statistics:");
            _logger.LogInformation("  📦 Nodes migrated:");
            _logger.LogInformation("    👥 Users: {UserCount}", graphUsers.Count);
            _logger.LogInformation("    🎭 Profiles: {ProfileCount}", graphProfiles.Count);
            _logger.LogInformation("    📝 WatchLists: {WatchListCount}", graphWatchLists.Count);
            _logger.LogInformation("    🎬 Media: {MediaCount}", graphMedias.Count);
            _logger.LogInformation("    📺 Episodes: {EpisodeCount}", graphEpisodes.Count);
            _logger.LogInformation("    🎭 Persons: {PersonCount}", graphPersons.Count);
            _logger.LogInformation("    🏷️ Genres: {GenreCount}", graphGenres.Count);
            _logger.LogInformation("    🎯 Roles: {RoleCount}", graphRoles.Count);
            _logger.LogInformation(
                "    💳 Subscriptions: {SubscriptionCount}",
                graphSubscriptions.Count
            );
            _logger.LogInformation("    🔐 Privileges: {PrivilegeCount}", graphPrivileges.Count);
            _logger.LogInformation("  🔗 Relationships migrated:");
            _logger.LogInformation(
                "    👥➡️🎭 User-Profile: {UserProfileCount}",
                userProfiles.Count()
            );
            _logger.LogInformation(
                "    🎭➡️📝 Profile-WatchList: {ProfileWatchListCount}",
                profileWatchLists.Count()
            );
            _logger.LogInformation(
                "    📝➡️🎬 WatchList-Media: {WatchListMediaCount}",
                watchListMedias.Count()
            );
            _logger.LogInformation(
                "    🎬➡️📺 Media-Episode: {MediaEpisodeCount}",
                mediaEpisodes.Count()
            );
            _logger.LogInformation(
                "    🎬➡️🏷️ Media-Genre: {MediaGenreCount}",
                mediaGenres.Count()
            );
            _logger.LogInformation(
                "    🎭➡️🎬 Person-Media-Role: {PersonMediaRoleCount}",
                personMediaRoles.Count()
            );
            _logger.LogInformation("    🎭➡️🎬 Reviews: {ReviewCount}", reviews.Count());
            _logger.LogInformation(
                "    👥➡️💳 User-Subscription: {UserSubscriptionCount}",
                userSubscriptions.Count()
            );
            _logger.LogInformation(
                "    💳➡️🏷️ Subscription-Genre: {SubscriptionGenreCount}",
                subscriptionGenres.Count()
            );
            _logger.LogInformation(
                "    👥➡️🔐 User-Privilege: {UserPrivilegeCount}",
                userPrivileges.Count()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Migration failed: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    public async Task TestPostgreSqlConnectionAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var postgreSqlService = scope.ServiceProvider.GetRequiredService<IPostgreSqlDataService>();
        var connected = await postgreSqlService.TestConnectionAsync();
        if (!connected)
        {
            throw new InvalidOperationException("PostgreSQL connection test failed");
        }
    }

    public async Task TestNeo4jConnectionAsync()
    {
        var connected = await _neo4jService.TestConnectionAsync();
        if (!connected)
        {
            throw new InvalidOperationException("Neo4j connection test failed");
        }
    }
}
