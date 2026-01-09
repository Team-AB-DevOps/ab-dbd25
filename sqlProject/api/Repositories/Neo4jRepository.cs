using api.ExceptionHandlers;
using api.Mappers;
using api.Models.DTOs.Domain;
using api.Repositories.Interfaces;
using Neo4j.Driver;

namespace api.Repositories;

public class Neo4jRepository(IDriver driver) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (m:Media)
                    OPTIONAL MATCH (m)-[:BELONGS_TO_GENRE]-(g:Genre)
                    OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                    OPTIONAL MATCH (m)-[r:WORKED_ON]-(p:Person)
                    RETURN m, 
                        collect(DISTINCT g) as genres,
                        collect(DISTINCT e) as episodes,
                        collect(DISTINCT {person: p, role: r.role}) as people
                "
                );

                var records = await cursor.ToListAsync();

                return records.Select(record => record.FromNeo4jRecordToDto()).ToList();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching media from Neo4j", ex);
        }
    }

    public async Task<MediaDto> GetMediaById(int id)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (m:Media) WHERE m.id = $id
                    OPTIONAL MATCH (m)-[:BELONGS_TO_GENRE]-(g:Genre)
                    OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                    OPTIONAL MATCH (m)-[r:WORKED_ON]-(p:Person)
                    RETURN m, 
                        collect(DISTINCT g) as genres,
                        collect(DISTINCT e) as episodes,
                        collect(DISTINCT {person: p, role: r.role}) as people
                ",
                    new { id }
                );

                var record = await cursor.SingleAsync();

                return record.FromNeo4jRecordToDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching media from Neo4j", ex);
        }
    }

    public async Task<List<MediaDto>> SearchMediasByName(string name)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (m:Media) WHERE toLower(m.name) CONTAINS toLower($name)
                    OPTIONAL MATCH (m)-[:BELONGS_TO_GENRE]-(g:Genre)
                    OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                    OPTIONAL MATCH (m)-[r:WORKED_ON]-(p:Person)
                    RETURN m, 
                        collect(DISTINCT g) as genres,
                        collect(DISTINCT e) as episodes,
                        collect(DISTINCT {person: p, role: r.role}) as people
                ",
                    new { name }
                );

                var records = await cursor.ToListAsync();

                return records.Select(record => record.FromNeo4jRecordToDto()).ToList();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error searching media from Neo4j", ex);
        }
    }

    public async Task<MediaDto> UpdateMedia(UpdateMediaDto updatedMedia, int id)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                MATCH (m:Media) WHERE m.id = $id
                SET
                  m.name = $name,
                  m.type = $type,
                  m.runtime = $runtime,
                  m.description = $description,
                  m.cover = $cover,
                  m.ageLimit = $ageLimit,
                  m.release = $release
                WITH m
                OPTIONAL MATCH (m)-[r:BELONGS_TO_GENRE]->(:Genre)
                DELETE r
                WITH m
                UNWIND $genres AS genreName
                MERGE (g:Genre {name: genreName})
                MERGE (m)-[:BELONGS_TO_GENRE]->(g)
                WITH m
                OPTIONAL MATCH (m)-[:BELONGS_TO_GENRE]-(g:Genre)
                OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                OPTIONAL MATCH (m)-[rel:WORKED_ON]-(p:Person)
                RETURN m,
                    collect(DISTINCT g) as genres,
                    collect(DISTINCT e) as episodes,
                    collect(DISTINCT {person: p, role: rel.role}) as people
                ",
                    new
                    {
                        id,
                        name = updatedMedia.Name,
                        type = updatedMedia.Type,
                        runtime = updatedMedia.Runtime,
                        description = updatedMedia.Description,
                        cover = updatedMedia.Cover,
                        ageLimit = updatedMedia.AgeLimit,
                        release = updatedMedia.Release.ToString("yyyy-MM-dd"),
                        genres = updatedMedia.Genres
                    }
                );

                var records = await cursor.ToListAsync();

                if (records.Count == 0)
                {
                    throw new NotFoundException($"Media not found with ID: {id}");
                }

                return records[0].FromNeo4jRecordToDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error updating media in Neo4j", ex);
        }
    }

    public async Task<MediaDto> CreateMedia(CreateMediaDto newMedia)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteWriteAsync(async tx =>
            {
                // Get the next available ID for Media nodes
                var nextId = await GetNextIdForLabel(tx, "Media");

                var cursor = await tx.RunAsync(
                    @"
                    CREATE (m:Media {
                        id: $id,
                        name: $name,
                        type: $type,
                        runtime: $runtime,
                        description: $description,
                        cover: $cover,
                        ageLimit: $ageLimit,
                        release: $release
                    })
                    WITH m
                    UNWIND $genres AS genreName
                    MERGE (g:Genre {name: genreName})
                    MERGE (m)-[:BELONGS_TO_GENRE]->(g)
                    WITH m
                    OPTIONAL MATCH (m)-[:BELONGS_TO_GENRE]-(g:Genre)
                    OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                    OPTIONAL MATCH (m)-[r:WORKED_ON]-(p:Person)
                    RETURN m, 
                        collect(DISTINCT g) as genres,
                        collect(DISTINCT e) as episodes,
                        collect(DISTINCT {person: p, role: r.role}) as people
                ",
                    new
                    {
                        id = nextId,
                        name = newMedia.Name,
                        type = newMedia.Type,
                        runtime = newMedia.Runtime,
                        description = newMedia.Description,
                        cover = newMedia.Cover,
                        ageLimit = newMedia.AgeLimit,
                        release = newMedia.Release.ToString("yyyy-MM-dd"),
                        genres = newMedia.Genres
                    }
                );

                var record = await cursor.SingleAsync();

                return record.FromNeo4jRecordToDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error creating media in Neo4j", ex);
        }
    }

    public async Task DeleteMediaById(int id)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            await session.ExecuteWriteAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    "MATCH (m:Media {id: $id}) DETACH DELETE m RETURN count(m) as deletedCount",
                    new { id }
                );
                var result = await cursor.SingleAsync();
                if (result["deletedCount"].As<int>() == 0)
                {
                    throw new NotFoundException($"Media not found with ID: {id}");
                }
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error deleting media from Neo4j", ex);
        }
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (m:Media) WHERE m.id = $id
                    OPTIONAL MATCH (m)-[:HAS_EPISODE]-(e:Episode)
                    RETURN e
                ",
                    new { id }
                );

                var records = await cursor.ToListAsync();

                // Filter out null episodes (happens when media has no episodes)
                var episodes = records
                    .Where(record => record["e"].As<INode>() != null)
                    .Select(record => record.FromNeo4jRecordToEpisodeDto())
                    .ToList();

                if (episodes.Count == 0)
                {
                    throw new NotFoundException($"No episodes found for media with ID: {id}");
                }

                return episodes;
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching episodes from Neo4j", ex);
        }
    }

    public async Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                MATCH (m:Media) WHERE m.id = $id
                MATCH (m)-[:HAS_EPISODE]-(e:Episode) WHERE e.id = $episodeId
                RETURN e
            ",
                    new { id, episodeId }
                );

                var records = await cursor.ToListAsync();

                if (records.Count == 0 || records[0]["e"].As<INode>() == null)
                {
                    throw new NotFoundException(
                        $"Episode with ID {episodeId} not found for media with ID: {id}"
                    );
                }

                return records[0].FromNeo4jRecordToEpisodeDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching episode from Neo4j", ex);
        }
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (u:User)
                    OPTIONAL MATCH (u)-[:SUBSCRIBES_TO]-(s:Subscription)
                    OPTIONAL MATCH (u)-[:HAS_PRIVILEGE]-(pr:Privilege)
                    OPTIONAL MATCH (u)-[:OWNS]-(p:Profile)
                    WITH u, s, pr, p,
                        [(p)-[rev:REVIEWED]-(rm:Media) | {media: rm, rating: rev.rating, description: rev.description}] as profileReviews,
                        [(p)-[:HAS_WATCHLIST]-(w:WatchList)-[:CONTAINS]-(wm:Media) | {watchlist: w, media: wm}] as profileWatchlists
                    RETURN u,
                        collect(DISTINCT s) as subscriptions,
                        collect(DISTINCT pr) as privileges,
                        collect(DISTINCT {profile: p, reviews: profileReviews, watchlists: profileWatchlists}) as profilesData
                "
                );

                var records = await cursor.ToListAsync();

                return records.Select(record => record.FromNeo4jRecordToUserDto()).ToList();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching users from Neo4j", ex);
        }
    }

    public async Task<UserDto> GetUserById(int id)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (u:User) WHERE u.id = $id
                    OPTIONAL MATCH (u)-[:SUBSCRIBES_TO]-(s:Subscription)
                    OPTIONAL MATCH (u)-[:HAS_PRIVILEGE]-(pr:Privilege)
                    OPTIONAL MATCH (u)-[:OWNS]-(p:Profile)
                    WITH u, s, pr, p,
                        [(p)-[rev:REVIEWED]-(rm:Media) | {media: rm, rating: rev.rating, description: rev.description}] as profileReviews,
                        [(p)-[:HAS_WATCHLIST]-(w:WatchList)-[:CONTAINS]-(wm:Media) | {watchlist: w, media: wm}] as profileWatchlists
                    RETURN u,
                        collect(DISTINCT s) as subscriptions,
                        collect(DISTINCT pr) as privileges,
                        collect(DISTINCT {profile: p, reviews: profileReviews, watchlists: profileWatchlists}) as profilesData
                ",
                    new { id }
                );

                var record = await cursor.SingleAsync();

                return record.FromNeo4jRecordToUserDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching users from Neo4j", ex);
        }
    }

    // TEST BY RUNNING:
    // MATCH (u:User) WHERE u.id = 1
    // MATCH (u)-[:OWNS]-(p:Profile) WHERE p.id = 1
    // MATCH (p)-[:HAS_WATCHLIST]-(w:WatchList)-[:CONTAINS]-(m:Media) WHERE m.id = 13
    // RETURN u, p, w, m
    public async Task AddMediaToWatchList(int userId, int profileId, int mediaId)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            await session.ExecuteWriteAsync(async tx =>
            {
                // Validate user, profile, and media exist, and check age restrictions
                var validationCursor = await tx.RunAsync(
                    @"
                    MATCH (u:User) WHERE u.id = $userId
                    MATCH (u)-[:OWNS]-(p:Profile) WHERE p.id = $profileId
                    MATCH (m:Media) WHERE m.id = $mediaId
                    RETURN u, p, m
                    ",
                    new
                    {
                        userId,
                        profileId,
                        mediaId
                    }
                );

                IRecord validationRecord;
                try
                {
                    validationRecord = await validationCursor.SingleAsync();
                }
                catch (Exception)
                {
                    throw new NotFoundException(
                        "User, profile, or media does not exist, or profile doesn't belong to user"
                    );
                }

                var profileNode = validationRecord["p"].As<INode>();
                var mediaNode = validationRecord["m"].As<INode>();

                // Check age restriction for child profiles
                var isChild = profileNode.Properties["isChild"].As<bool>();
                var ageLimit = mediaNode.Properties["ageLimit"].As<int?>() ?? null;

                if (isChild && ageLimit >= 18)
                {
                    throw new BadRequestException(
                        "Media age restriction doesn't allow adding to child profile"
                    );
                }

                // Get or create watchlist and add media
                await tx.RunAsync(
                    @"
                    MATCH (p:Profile) WHERE p.id = $profileId
                    MERGE (p)-[:HAS_WATCHLIST]->(w:WatchList)
                    WITH w
                    MATCH (m:Media) WHERE m.id = $mediaId
                    MERGE (w)-[:CONTAINS]->(m)
                    ",
                    new { profileId, mediaId }
                );
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error adding media to watchlist in Neo4j", ex);
        }
    }

    public async Task<UserDto> CreateProfile(int userId, CreateProfileDto createProfileDto)
    {
        await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

        try
        {
            return await session.ExecuteWriteAsync(async tx =>
            {
                // Verify user exists
                var userCheckCursor = await tx.RunAsync(
                    "MATCH (u:User) WHERE u.id = $userId RETURN u",
                    new { userId }
                );

                if (!await userCheckCursor.FetchAsync())
                {
                    throw new NotFoundException($"User with ID {userId} not found");
                }

                // Get next profile ID
                var profileId = await GetNextIdForLabel(tx, "Profile");

                // Create profile node and return updated user
                var cursor = await tx.RunAsync(
                    @"
                    MATCH (u:User) WHERE u.id = $userId
                    CREATE (p:Profile {id: $profileId, name: $name, isChild: $isChild})
                    CREATE (w:WatchList {id: $profileId, isLocked: false})
                    CREATE (u)-[:OWNS]->(p)
                    CREATE (p)-[:HAS_WATCHLIST]->(w)
                    WITH u
                    OPTIONAL MATCH (u)-[:SUBSCRIBES_TO]-(s:Subscription)
                    OPTIONAL MATCH (u)-[:HAS_PRIVILEGE]-(pr:Privilege)
                    OPTIONAL MATCH (u)-[:OWNS]-(p2:Profile)
                    WITH u, s, pr, p2,
                        [(p2)-[rev:REVIEWED]-(rm:Media) | {media: rm, rating: rev.rating, description: rev.description}] as profileReviews,
                        [(p2)-[:HAS_WATCHLIST]-(wl:WatchList)-[:CONTAINS]-(wm:Media) | {watchlist: wl, media: wm}] as profileWatchlists
                    RETURN u,
                        collect(DISTINCT s) as subscriptions,
                        collect(DISTINCT pr) as privileges,
                        collect(DISTINCT {profile: p2, reviews: profileReviews, watchlists: profileWatchlists}) as profilesData
                    ",
                    new
                    {
                        userId,
                        profileId,
                        name = createProfileDto.Name,
                        isChild = createProfileDto.IsChild
                    }
                );

                var record = await cursor.SingleAsync();
                return record.FromNeo4jRecordToUserDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error creating profile in Neo4j", ex);
        }
    }

    private async Task<int> GetNextIdForLabel(IAsyncQueryRunner tx, string label)
    {
        var cursor = await tx.RunAsync(
            $"MATCH (n:{label}) WHERE n.id IS NOT NULL RETURN COALESCE(MAX(n.id), 0) AS maxId"
        );

        var record = await cursor.SingleAsync();
        var maxId = record["maxId"].As<int>();

        return maxId + 1;
    }
}