using api.ExceptionHandlers;
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

    public async Task<MediaDto> GetMediaById(int id)
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");
        var filter = Builders<MongoMedia>.Filter.Eq(m => m.Id, id);
        var result = await mediaCollection.Find(filter).SingleOrDefaultAsync();

        if (result == null)
        {
            throw new NotFoundException("Media not found");
        }

        return result.FromMongoEntityToDto();
    }

    public async Task<MediaDto> UpdateMedia(UpdateMediaDto updatedMedia, int id)
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");
        var filter = Builders<MongoMedia>.Filter.Eq(m => m.Id, id);

        var media = Builders<MongoMedia>.Update
            .Set("name", updatedMedia.Name)
            .Set("type", updatedMedia.Type)
            .Set("runtime", updatedMedia.Runtime)
            .Set("description", updatedMedia.Description)
            .Set("cover", updatedMedia.Cover)
            .Set("ageLimit", updatedMedia.AgeLimit)
            .Set("release", updatedMedia.Release.ToString("yyyy-MM-dd"))
            .Set("genres", updatedMedia.Genres.ToList());

        var options = new FindOneAndUpdateOptions<MongoMedia>
        {
            ReturnDocument = ReturnDocument.After
        };

        var result = await mediaCollection.FindOneAndUpdateAsync(filter, media, options);

        if (result == null)
        {
            throw new NotFoundException("Media not found");
        }

        return result.FromMongoEntityToDto();
    }

    public async Task<MediaDto> CreateMedia(CreateMediaDto newMedia)
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");

        var newMongoMedia = new MongoMedia
        {
            Name = newMedia.Name,
            Type = newMedia.Type,
            Runtime = newMedia.Runtime,
            Description = newMedia.Description,
            Cover = newMedia.Cover,
            AgeLimit = newMedia.AgeLimit,
            Release = newMedia.Release.ToString("yyyy-MM-dd"),
            Genres = newMedia.Genres.ToList()
        };

        await mediaCollection.InsertOneAsync(newMongoMedia);

        return newMongoMedia.FromMongoEntityToDto();
    }

    public async Task DeleteMediaById(int id)
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");
        var filter = Builders<MongoMedia>.Filter.Eq(m => m.Id, id);


        var result = await mediaCollection.FindOneAndDeleteAsync(filter);

        if (result == null)
        {
            throw new NotFoundException("Media not found");
        }
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(int id)
    {
        var media = await GetMediaById(id);

        var filter = Builders<MongoEpisode>.Filter.In("_id", media.Episodes);

        var episodeCollection = database.GetCollection<MongoEpisode>("episodes");

        var result = await episodeCollection.Find(filter).ToListAsync();

        if (result is null)
        {
            throw new NotFoundException(
                $"No episode found with ID: {media.Id}, for media with ID: {id}"
            );
        }

        return result.Select(e => e.FromMongoEntityToDto()).ToList();
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