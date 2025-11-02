using api.DTOs;
using api.Mappers;
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
                MATCH (media:Media) 
                RETURN media
            "
                );

                var records = await cursor.ToListAsync();

                return records
                    .Select(record => record["media"].As<INode>().FromNeo4JEntityToDto())
                    .ToList();
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching media from Neo4j", ex);
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
}
