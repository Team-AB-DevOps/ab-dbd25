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
                    RETURN m, g, e, p
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
                    RETURN m, g, e, p
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

    public Task<MediaDto> CreateMedia(CreateMediaDto newMedia)
    {
        throw new NotImplementedException();
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
}
