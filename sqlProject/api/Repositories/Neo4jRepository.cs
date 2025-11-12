using api.DTOs;
using api.Models.DTOs.Domain;
using Neo4j.Driver;

namespace api.Repositories;

public class Neo4jRepository(IDriver driver) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        // await using var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
        //
        // try
        // {
        //     return await session.ExecuteReadAsync(async tx =>
        //     {
        //         var cursor = await tx.RunAsync(
        //             @"
        //         MATCH (media:Media)
        //         RETURN media
        //     "
        //         );
        //
        //         var records = await cursor.ToListAsync();
        //
        //         return records
        //             .Select(record => record["media"].As<INode>().FromNeo4JEntityToDto())
        //             .ToList();
        //     });
        // }
        // catch (Neo4jException ex)
        // {
        //     throw new Exception("Error fetching media from Neo4j", ex);
        // }

        throw new NotImplementedException();
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
