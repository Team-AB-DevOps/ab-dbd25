using api.DTOs;
using api.Models.DTOs.Domain;
using MongoDB.Driver;

namespace api.Repositories;

public class MongoRepository(IMongoDatabase database) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        // var collection = database.GetCollection<BsonDocument>("medias");
        // var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
        //
        // return documents.Select(doc => doc.FromMongoEntityToDto()).ToList();

        throw new NotImplementedException();
    }

    public Task<MediaDto> GetMediaById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<MediaDto> UpdateMedia(MediaDto updatedMedia, int id)
    {
        throw new NotImplementedException();
    }

    public Task<MediaDto> CreateMedia(CreateMediaDto newMedia)
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
}
