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
                
                return records.FromNeo4jRecordsToDto();
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
                ", new { id }
                );
        
                var records = await cursor.ToListAsync();
                
                return records.FromNeo4jRecordsToDto().First();
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
                ", new { 
                    id = nextId,
                    name = newMedia.Name,
                    type = newMedia.Type,
                    runtime = newMedia.Runtime,
                    description = newMedia.Description,
                    cover = newMedia.Cover,
                    ageLimit = newMedia.AgeLimit,
                    release = newMedia.Release.ToString("yyyy-MM-dd"),
                    genres = newMedia.Genres
                });
        
                var records = await cursor.ToListAsync();
                
                return records.FromNeo4jRecordsToDto().First();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error creating media in Neo4j", ex);
        }
    }

    public Task DeleteMediaById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        throw new NotImplementedException();
    }

    public Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId)
    {
        throw new NotImplementedException();
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
