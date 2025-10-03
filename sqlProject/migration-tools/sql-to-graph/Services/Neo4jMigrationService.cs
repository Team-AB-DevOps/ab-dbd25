using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using SqlToGraph.Models;

namespace SqlToGraph.Services;

public interface INeo4jMigrationService
{
    Task<bool> TestConnectionAsync();
    Task InitializeDatabaseAsync();
    Task MigrateNodesAsync(
        IEnumerable<GraphUser> users,
        IEnumerable<GraphProfile> profiles,
        IEnumerable<GraphWatchList> watchLists,
        IEnumerable<GraphMedia> medias,
        IEnumerable<GraphEpisode> episodes,
        IEnumerable<GraphPerson> persons,
        IEnumerable<GraphGenre> genres,
        IEnumerable<GraphRole> roles,
        IEnumerable<GraphSubscription> subscriptions,
        IEnumerable<GraphPrivilege> privileges
    );
    Task MigrateRelationshipsAsync(
        IEnumerable<(int UserId, int ProfileId)> userProfiles,
        IEnumerable<(int ProfileId, int WatchListId)> profileWatchLists,
        IEnumerable<(int WatchListId, int MediaId)> watchListMedias,
        IEnumerable<(int MediaId, int EpisodeId)> mediaEpisodes,
        IEnumerable<(int MediaId, int GenreId)> mediaGenres,
        IEnumerable<(int PersonId, int MediaId, string Role)> personMediaRoles,
        IEnumerable<(int ProfileId, int MediaId, int Rating, string ReviewText, DateTime ReviewDate)> reviews,
        IEnumerable<(int UserId, int SubscriptionId)> userSubscriptions,
        IEnumerable<(int SubscriptionId, int GenreId)> subscriptionGenres,
        IEnumerable<(int UserId, int PrivilegeId)> userPrivileges
    );
}

public class Neo4jMigrationService : INeo4jMigrationService, IDisposable
{
    private readonly IDriver _driver;
    private readonly ILogger<Neo4jMigrationService> _logger;
    private readonly int _batchSize;

    public Neo4jMigrationService(IConfiguration configuration, ILogger<Neo4jMigrationService> logger)
    {
        _logger = logger;
        
        var neo4jConfig = configuration.GetSection("Neo4j");
        var uri = neo4jConfig["Uri"] ?? throw new ArgumentNullException("Neo4j:Uri is required");
        var user = neo4jConfig["User"] ?? throw new ArgumentNullException("Neo4j:User is required");
        var password = neo4jConfig["Password"] ?? throw new ArgumentNullException("Neo4j:Password is required");
        
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        _batchSize = configuration.GetValue<int>("Migration:BatchSize", 1000);
        
        _logger.LogInformation("Connected to Neo4j database at {Uri}", uri);
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _driver.VerifyConnectivityAsync();
            _logger.LogInformation("Neo4j connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Neo4j connection test failed");
            return false;
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        _logger.LogInformation("Initializing Neo4j database schema...");
        
        using var session = _driver.AsyncSession();
        
        // Clear existing data (optional - comment out if you want to preserve existing data)
        await session.RunAsync("MATCH (n) DETACH DELETE n");
        _logger.LogInformation("Cleared existing data");
        
        // Create constraints
        var constraints = new[]
        {
            "CREATE CONSTRAINT user_id IF NOT EXISTS FOR (u:User) REQUIRE u.id IS UNIQUE",
            "CREATE CONSTRAINT profile_id IF NOT EXISTS FOR (p:Profile) REQUIRE p.id IS UNIQUE",
            "CREATE CONSTRAINT watchlist_id IF NOT EXISTS FOR (w:WatchList) REQUIRE w.id IS UNIQUE",
            "CREATE CONSTRAINT media_id IF NOT EXISTS FOR (m:Media) REQUIRE m.id IS UNIQUE",
            "CREATE CONSTRAINT episode_id IF NOT EXISTS FOR (e:Episode) REQUIRE e.id IS UNIQUE",
            "CREATE CONSTRAINT person_id IF NOT EXISTS FOR (p:Person) REQUIRE p.id IS UNIQUE",
            "CREATE CONSTRAINT genre_id IF NOT EXISTS FOR (g:Genre) REQUIRE g.id IS UNIQUE",
            "CREATE CONSTRAINT role_id IF NOT EXISTS FOR (r:Role) REQUIRE r.id IS UNIQUE",
            "CREATE CONSTRAINT subscription_id IF NOT EXISTS FOR (s:Subscription) REQUIRE s.id IS UNIQUE",
            "CREATE CONSTRAINT privilege_id IF NOT EXISTS FOR (priv:Privilege) REQUIRE priv.id IS UNIQUE"
        };

        foreach (var constraint in constraints)
        {
            try
            {
                await session.RunAsync(constraint);
                _logger.LogDebug("Created constraint: {Constraint}", constraint);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create constraint: {Constraint}", constraint);
            }
        }

        // Create indexes for performance
        var indexes = new[]
        {
            "CREATE INDEX user_email IF NOT EXISTS FOR (u:User) ON (u.email)",
            "CREATE INDEX profile_name IF NOT EXISTS FOR (p:Profile) ON (p.name)",
            "CREATE INDEX media_name IF NOT EXISTS FOR (m:Media) ON (m.name)",
            "CREATE INDEX media_type IF NOT EXISTS FOR (m:Media) ON (m.type)",
            "CREATE INDEX episode_name IF NOT EXISTS FOR (e:Episode) ON (e.name)",
            "CREATE INDEX person_name IF NOT EXISTS FOR (p:Person) ON (p.firstName, p.lastName)",
            "CREATE INDEX genre_name IF NOT EXISTS FOR (g:Genre) ON (g.name)",
            "CREATE INDEX role_name IF NOT EXISTS FOR (r:Role) ON (r.name)",
            "CREATE INDEX subscription_name IF NOT EXISTS FOR (s:Subscription) ON (s.name)",
            "CREATE INDEX privilege_name IF NOT EXISTS FOR (priv:Privilege) ON (priv.name)"
        };

        foreach (var index in indexes)
        {
            try
            {
                await session.RunAsync(index);
                _logger.LogDebug("Created index: {Index}", index);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create index: {Index}", index);
            }
        }

        _logger.LogInformation("Database schema initialization completed");
    }

    public async Task MigrateNodesAsync(
        IEnumerable<GraphUser> users,
        IEnumerable<GraphProfile> profiles,
        IEnumerable<GraphWatchList> watchLists,
        IEnumerable<GraphMedia> medias,
        IEnumerable<GraphEpisode> episodes,
        IEnumerable<GraphPerson> persons,
        IEnumerable<GraphGenre> genres,
        IEnumerable<GraphRole> roles,
        IEnumerable<GraphSubscription> subscriptions,
        IEnumerable<GraphPrivilege> privileges)
    {
        _logger.LogInformation("Starting node migration...");

        await MigrateUsersAsync(users);
        await MigrateProfilesAsync(profiles);
        await MigrateWatchListsAsync(watchLists);
        await MigrateMediasAsync(medias);
        await MigrateEpisodesAsync(episodes);
        await MigratePersonsAsync(persons);
        await MigrateGenresAsync(genres);
        await MigrateRolesAsync(roles);
        await MigrateSubscriptionsAsync(subscriptions);
        await MigratePrivilegesAsync(privileges);

        _logger.LogInformation("Node migration completed successfully");
    }

    private async Task MigrateUsersAsync(IEnumerable<GraphUser> users)
    {
        const string cypher = @"
            UNWIND $users AS user
            CREATE (u:User {
                id: user.id,
                firstName: user.firstName,
                lastName: user.lastName,
                email: user.email,
                password: user.password
            })";

        // Convert GraphUser objects to dictionaries that Neo4j can understand
        var userDictionaries = users.Select(u => new Dictionary<string, object>
        {
            ["id"] = u.Id,
            ["firstName"] = u.FirstName,
            ["lastName"] = u.LastName,
            ["email"] = u.Email,
            ["password"] = u.Password
        }).ToList();

        await ExecuteBatchedCypher(cypher, userDictionaries, "users", "User");
    }

    private async Task MigrateProfilesAsync(IEnumerable<GraphProfile> profiles)
    {
        const string cypher = @"
            UNWIND $profiles AS profile
            CREATE (p:Profile {
                id: profile.id,
                name: profile.name,
                isChild: profile.isChild
            })";

        // Convert GraphProfile objects to dictionaries that Neo4j can understand
        var profileDictionaries = profiles.Select(p => new Dictionary<string, object>
        {
            ["id"] = p.Id,
            ["name"] = p.Name,
            ["isChild"] = p.IsChild
        }).ToList();

        await ExecuteBatchedCypher(cypher, profileDictionaries, "profiles", "Profile");
    }

    private async Task MigrateWatchListsAsync(IEnumerable<GraphWatchList> watchLists)
    {
        const string cypher = @"
            UNWIND $watchLists AS watchList
            CREATE (w:WatchList {
                id: watchList.id,
                isLocked: watchList.isLocked
            })";

        // Convert GraphWatchList objects to dictionaries that Neo4j can understand
        var watchListDictionaries = watchLists.Select(w => new Dictionary<string, object>
        {
            ["id"] = w.Id,
            ["isLocked"] = w.IsLocked
        }).ToList();

        await ExecuteBatchedCypher(cypher, watchListDictionaries, "watchLists", "WatchList");
    }

    private async Task MigrateMediasAsync(IEnumerable<GraphMedia> medias)
    {
        const string cypher = @"
            UNWIND $medias AS media
            CREATE (m:Media {
                id: media.id,
                name: media.name,
                type: media.type,
                runtime: media.runtime,
                description: media.description,
                cover: media.cover,
                ageLimit: media.ageLimit,
                release: date(media.release)
            })";

        // Convert GraphMedia objects to dictionaries that Neo4j can understand
        var mediaDictionaries = medias.Select(m => new Dictionary<string, object>
        {
            ["id"] = m.Id,
            ["name"] = m.Name,
            ["type"] = m.Type,
            ["runtime"] = m.Runtime,
            ["description"] = m.Description,
            ["cover"] = m.Cover,
            ["ageLimit"] = m.AgeLimit ?? (object)DBNull.Value,
            ["release"] = m.Release.ToString("yyyy-MM-dd")
        }).ToList();

        await ExecuteBatchedCypher(cypher, mediaDictionaries, "medias", "Media");
    }

    private async Task MigrateEpisodesAsync(IEnumerable<GraphEpisode> episodes)
    {
        const string cypher = @"
            UNWIND $episodes AS episode
            CREATE (e:Episode {
                id: episode.id,
                name: episode.name,
                seasonCount: episode.seasonCount,
                episodeCount: episode.episodeCount,
                runtime: episode.runtime,
                description: episode.description,
                release: date(episode.release)
            })";

        // Convert GraphEpisode objects to dictionaries that Neo4j can understand
        var episodeDictionaries = episodes.Select(e => new Dictionary<string, object>
        {
            ["id"] = e.Id,
            ["name"] = e.Name,
            ["seasonCount"] = e.SeasonCount ?? (object)DBNull.Value,
            ["episodeCount"] = e.EpisodeCount,
            ["runtime"] = e.Runtime,
            ["description"] = e.Description,
            ["release"] = e.Release.ToString("yyyy-MM-dd")
        }).ToList();

        await ExecuteBatchedCypher(cypher, episodeDictionaries, "episodes", "Episode");
    }

    private async Task MigratePersonsAsync(IEnumerable<GraphPerson> persons)
    {
        const string cypher = @"
            UNWIND $persons AS person
            CREATE (p:Person {
                id: person.id,
                firstName: person.firstName,
                lastName: person.lastName,
                birthDate: date(person.birthDate),
                gender: person.gender
            })";

        // Convert GraphPerson objects to dictionaries that Neo4j can understand
        var personDictionaries = persons.Select(p => new Dictionary<string, object>
        {
            ["id"] = p.Id,
            ["firstName"] = p.FirstName,
            ["lastName"] = p.LastName,
            ["birthDate"] = p.BirthDate.ToString("yyyy-MM-dd"),
            ["gender"] = p.Gender
        }).ToList();

        await ExecuteBatchedCypher(cypher, personDictionaries, "persons", "Person");
    }

    private async Task MigrateGenresAsync(IEnumerable<GraphGenre> genres)
    {
        const string cypher = @"
            UNWIND $genres AS genre
            CREATE (g:Genre {
                id: genre.id,
                name: genre.name
            })";

        // Convert GraphGenre objects to dictionaries that Neo4j can understand
        var genreDictionaries = genres.Select(g => new Dictionary<string, object>
        {
            ["id"] = g.Id,
            ["name"] = g.Name
        }).ToList();

        await ExecuteBatchedCypher(cypher, genreDictionaries, "genres", "Genre");
    }

    private async Task MigrateRolesAsync(IEnumerable<GraphRole> roles)
    {
        const string cypher = @"
            UNWIND $roles AS role
            CREATE (r:Role {
                id: role.id,
                name: role.name
            })";

        // Convert GraphRole objects to dictionaries that Neo4j can understand
        var roleDictionaries = roles.Select(r => new Dictionary<string, object>
        {
            ["id"] = r.Id,
            ["name"] = r.Name
        }).ToList();

        await ExecuteBatchedCypher(cypher, roleDictionaries, "roles", "Role");
    }

    private async Task MigrateSubscriptionsAsync(IEnumerable<GraphSubscription> subscriptions)
    {
        const string cypher = @"
            UNWIND $subscriptions AS subscription
            CREATE (s:Subscription {
                id: subscription.id,
                name: subscription.name,
                price: subscription.price
            })";

        // Convert GraphSubscription objects to dictionaries that Neo4j can understand
        var subscriptionDictionaries = subscriptions.Select(s => new Dictionary<string, object>
        {
            ["id"] = s.Id,
            ["name"] = s.Name,
            ["price"] = s.Price
        }).ToList();

        await ExecuteBatchedCypher(cypher, subscriptionDictionaries, "subscriptions", "Subscription");
    }

    private async Task MigratePrivilegesAsync(IEnumerable<GraphPrivilege> privileges)
    {
        const string cypher = @"
            UNWIND $privileges AS privilege
            CREATE (priv:Privilege {
                id: privilege.id,
                name: privilege.name
            })";

        // Convert GraphPrivilege objects to dictionaries that Neo4j can understand
        var privilegeDictionaries = privileges.Select(p => new Dictionary<string, object>
        {
            ["id"] = p.Id,
            ["name"] = p.Name
        }).ToList();

        await ExecuteBatchedCypher(cypher, privilegeDictionaries, "privileges", "Privilege");
    }

    public async Task MigrateRelationshipsAsync(
        IEnumerable<(int UserId, int ProfileId)> userProfiles,
        IEnumerable<(int ProfileId, int WatchListId)> profileWatchLists,
        IEnumerable<(int WatchListId, int MediaId)> watchListMedias,
        IEnumerable<(int MediaId, int EpisodeId)> mediaEpisodes,
        IEnumerable<(int MediaId, int GenreId)> mediaGenres,
        IEnumerable<(int PersonId, int MediaId, string Role)> personMediaRoles,
        IEnumerable<(int ProfileId, int MediaId, int Rating, string ReviewText, DateTime ReviewDate)> reviews,
        IEnumerable<(int UserId, int SubscriptionId)> userSubscriptions,
        IEnumerable<(int SubscriptionId, int GenreId)> subscriptionGenres,
        IEnumerable<(int UserId, int PrivilegeId)> userPrivileges)
    {
        _logger.LogInformation("Starting relationship migration...");

        await MigrateUserProfileRelationshipsAsync(userProfiles);
        await MigrateProfileWatchListRelationshipsAsync(profileWatchLists);
        await MigrateWatchListMediaRelationshipsAsync(watchListMedias);
        await MigrateMediaEpisodeRelationshipsAsync(mediaEpisodes);
        await MigrateMediaGenreRelationshipsAsync(mediaGenres);
        await MigratePersonMediaRoleRelationshipsAsync(personMediaRoles);
        await MigrateReviewRelationshipsAsync(reviews);
        await MigrateUserSubscriptionRelationshipsAsync(userSubscriptions);
        await MigrateSubscriptionGenreRelationshipsAsync(subscriptionGenres);
        await MigrateUserPrivilegeRelationshipsAsync(userPrivileges);

        _logger.LogInformation("Relationship migration completed successfully");
    }

    private async Task MigrateUserProfileRelationshipsAsync(IEnumerable<(int UserId, int ProfileId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (u:User {id: rel.userId})
            MATCH (p:Profile {id: rel.profileId})
            CREATE (u)-[:OWNS]->(p)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["userId"] = r.UserId,
            ["profileId"] = r.ProfileId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "OWNS");
    }

    private async Task MigrateProfileWatchListRelationshipsAsync(IEnumerable<(int ProfileId, int WatchListId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (p:Profile {id: rel.profileId})
            MATCH (w:WatchList {id: rel.watchListId})
            CREATE (p)-[:HAS_WATCHLIST]->(w)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["profileId"] = r.ProfileId,
            ["watchListId"] = r.WatchListId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "HAS_WATCHLIST");
    }

    private async Task MigrateWatchListMediaRelationshipsAsync(IEnumerable<(int WatchListId, int MediaId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (w:WatchList {id: rel.watchListId})
            MATCH (m:Media {id: rel.mediaId})
            CREATE (w)-[:CONTAINS {addedAt: datetime()}]->(m)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["watchListId"] = r.WatchListId,
            ["mediaId"] = r.MediaId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "CONTAINS");
    }

    private async Task MigrateMediaEpisodeRelationshipsAsync(IEnumerable<(int MediaId, int EpisodeId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (m:Media {id: rel.mediaId})
            MATCH (e:Episode {id: rel.episodeId})
            CREATE (m)-[:HAS_EPISODE]->(e)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["mediaId"] = r.MediaId,
            ["episodeId"] = r.EpisodeId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "HAS_EPISODE");
    }

    private async Task MigrateMediaGenreRelationshipsAsync(IEnumerable<(int MediaId, int GenreId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (m:Media {id: rel.mediaId})
            MATCH (g:Genre {id: rel.genreId})
            CREATE (m)-[:BELONGS_TO_GENRE]->(g)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["mediaId"] = r.MediaId,
            ["genreId"] = r.GenreId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "BELONGS_TO_GENRE");
    }

    private async Task MigratePersonMediaRoleRelationshipsAsync(IEnumerable<(int PersonId, int MediaId, string Role)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (p:Person {id: rel.personId})
            MATCH (m:Media {id: rel.mediaId})
            CREATE (p)-[:WORKED_ON {role: rel.role}]->(m)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["personId"] = r.PersonId,
            ["mediaId"] = r.MediaId,
            ["role"] = r.Role
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "WORKED_ON");
    }

    private async Task MigrateReviewRelationshipsAsync(IEnumerable<(int ProfileId, int MediaId, int Rating, string ReviewText, DateTime ReviewDate)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (p:Profile {id: rel.profileId})
            MATCH (m:Media {id: rel.mediaId})
            CREATE (p)-[:REVIEWED {
                rating: rel.rating,
                description: rel.reviewText,
                createdAt: datetime(rel.reviewDate)
            }]->(m)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["profileId"] = r.ProfileId,
            ["mediaId"] = r.MediaId,
            ["rating"] = r.Rating,
            ["reviewText"] = r.ReviewText,
            ["reviewDate"] = r.ReviewDate.ToString("yyyy-MM-ddTHH:mm:ss")
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "REVIEWED");
    }

    private async Task MigrateUserSubscriptionRelationshipsAsync(IEnumerable<(int UserId, int SubscriptionId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (u:User {id: rel.userId})
            MATCH (s:Subscription {id: rel.subscriptionId})
            CREATE (u)-[:SUBSCRIBES_TO]->(s)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["userId"] = r.UserId,
            ["subscriptionId"] = r.SubscriptionId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "SUBSCRIBES_TO");
    }

    private async Task MigrateSubscriptionGenreRelationshipsAsync(IEnumerable<(int SubscriptionId, int GenreId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (s:Subscription {id: rel.subscriptionId})
            MATCH (g:Genre {id: rel.genreId})
            CREATE (s)-[:GIVES_ACCESS_TO]->(g)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["subscriptionId"] = r.SubscriptionId,
            ["genreId"] = r.GenreId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "GIVES_ACCESS_TO");
    }

    private async Task MigrateUserPrivilegeRelationshipsAsync(IEnumerable<(int UserId, int PrivilegeId)> relationships)
    {
        const string cypher = @"
            UNWIND $relationships AS rel
            MATCH (u:User {id: rel.userId})
            MATCH (p:Privilege {id: rel.privilegeId})
            CREATE (u)-[:HAS_PRIVILEGE]->(p)";

        var data = relationships.Select(r => new Dictionary<string, object>
        {
            ["userId"] = r.UserId,
            ["privilegeId"] = r.PrivilegeId
        }).ToList();
        
        await ExecuteBatchedCypher(cypher, data, "relationships", "HAS_PRIVILEGE");
    }

    private async Task ExecuteBatchedCypher<T>(string cypher, List<T> data, string parameterName, string entityType)
    {
        if (!data.Any())
        {
            _logger.LogInformation("No {EntityType} data to migrate", entityType);
            return;
        }

        _logger.LogInformation("Migrating {Count} {EntityType} entities...", data.Count, entityType);

        using var session = _driver.AsyncSession();
        
        var batches = data.Chunk(_batchSize);
        int totalProcessed = 0;

        foreach (var batch in batches)
        {
            var parameters = new Dictionary<string, object> { [parameterName] = batch };
            
            try
            {
                await session.RunAsync(cypher, parameters);
                totalProcessed += batch.Length;
                _logger.LogDebug("Processed {Processed}/{Total} {EntityType} entities", totalProcessed, data.Count, entityType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to migrate batch of {EntityType} entities", entityType);
                throw;
            }
        }

        _logger.LogInformation("Successfully migrated {Count} {EntityType} entities", data.Count, entityType);
    }

    // Overloaded version for Dictionary data that Neo4j can understand
    private async Task ExecuteBatchedCypher(string cypher, List<Dictionary<string, object>> data, string parameterName, string entityType)
    {
        if (!data.Any())
        {
            _logger.LogInformation("No {EntityType} data to migrate", entityType);
            return;
        }

        _logger.LogInformation("Migrating {Count} {EntityType} entities...", data.Count, entityType);

        using var session = _driver.AsyncSession();
        
        var batches = data.Chunk(_batchSize);
        int totalProcessed = 0;

        foreach (var batch in batches)
        {
            var parameters = new Dictionary<string, object> { [parameterName] = batch };
            
            try
            {
                await session.RunAsync(cypher, parameters);
                totalProcessed += batch.Length;
                _logger.LogDebug("Processed {Processed}/{Total} {EntityType} entities", totalProcessed, data.Count, entityType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to migrate batch of {EntityType} entities", entityType);
                throw;
            }
        }

        _logger.LogInformation("Successfully migrated {Count} {EntityType} entities", data.Count, entityType);
    }

    public void Dispose()
    {
        _driver?.Dispose();
    }
}
