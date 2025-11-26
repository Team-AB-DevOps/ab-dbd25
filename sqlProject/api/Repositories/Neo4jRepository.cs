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
                    MATCH (m:Media)-[:BELONGS_TO_GENRE]->(g:Genre),
                        (m)-[:HAS_EPISODE]->(e:Episode),
                        (p:Person)-[:WORKED_ON]->(m)
                    RETURN m, e, g, p
                "
                );
        
                var records = await cursor.ToListAsync();
                
                if (!records.Any())
                    return new List<MediaDto>();
                
                // Group records by media ID to aggregate related data
                var mediaGroups = records.GroupBy(r => r["m"].As<INode>().Properties["id"].As<int>());
                
                var mediaDtos = new List<MediaDto>();
                
                foreach (var group in mediaGroups)
                {
                    var recordsList = group.ToList();
                    
                    // Aggregate episodes, genres, and persons for this media
                    var episodesList = recordsList.Select(r => r["e"].As<INode>()).ToList();
                    var genresList = recordsList.Select(r => r["g"].As<INode>()).ToList();
                    var personsList = recordsList.Select(r => r["p"].As<INode>()).ToList();
                    
                    // Convert to dictionary format
                    var recordDict = new Dictionary<string, object>
                    {
                        ["m"] = recordsList.First()["m"].As<INode>(),
                        ["episodes"] = episodesList,
                        ["genres"] = genresList,
                        ["persons"] = personsList
                    };
            
                    // Convert to MediaDto using the mapper
                    mediaDtos.Add(recordDict.FromNeo4jRecordToDto());
                }
                
                return mediaDtos;
            });
        }
        catch (Neo4jException ex)
        {
            throw new Exception("Error fetching media from Neo4j", ex);
        }
    }

    public Task<MediaDto> GetMediaById(int id)
    {
        throw new NotImplementedException();
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
