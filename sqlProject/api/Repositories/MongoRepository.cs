using api.Mappers;
using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Repositories.Interfaces;
using MongoDB.Driver;

namespace api.Repositories;

public class MongoRepository(IMongoDatabase database) : IRepository
{
    public async Task<List<MediaDto>> GetAllMedias()
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");
        var filter = Builders<MongoMedia>.Filter.Empty;
        var results = await mediaCollection.Find(filter).ToListAsync();

        return results?.Select(doc => doc.FromMongoEntityToDto()).ToList() ?? [];
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