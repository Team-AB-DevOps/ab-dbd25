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

    public Task<MediaDto> UpdateMedia(UpdateMediaDto updatedMedia, int id)
    {
        throw new NotImplementedException();
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
                await tx.RunAsync(
                    "MATCH (m:Media {id: $id}) DETACH DELETE m",
                    new { id }
                );
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
                    throw new NotFoundException($"Episode with ID {episodeId} not found for media with ID: {id}");
                }

                return records[0].FromNeo4jRecordToEpisodeDto();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching episode from Neo4j", ex);
        }
    }

    public Task<List<UserDto>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddMediaToWatchList(int userId, int profileId, int mediaId)
    {
        throw new NotImplementedException();
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