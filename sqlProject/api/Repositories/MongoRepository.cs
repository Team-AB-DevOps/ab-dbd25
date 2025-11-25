using api.ExceptionHandlers;
using api.Mappers;
using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Repositories;

public class MongoRepository(IMongoDatabase database) : IRepository
{
    private async Task<int> GetNextSequenceValue(string sequenceName)
    {
        var countersCollection = database.GetCollection<BsonDocument>("counters");
        var filter = Builders<BsonDocument>.Filter.Eq("_id", sequenceName);
        
        // Try to increment counter and get next value
        var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };
        
        var result = await countersCollection.FindOneAndUpdateAsync(filter, update, options);
        
        // If counter was just created via upsert, it starts at 1
        // Check if this conflicts with existing data
        if (result["sequence_value"].AsInt32 == 1)
        {
            var maxId = await GetMaxIdForSequence(sequenceName);
            if (maxId > 0)
            {
                // Reset counter to max ID and increment
                var setUpdate = Builders<BsonDocument>.Update.Set("sequence_value", maxId + 1);
                var setResult = await countersCollection.FindOneAndUpdateAsync(filter, setUpdate, 
                    new FindOneAndUpdateOptions<BsonDocument> { ReturnDocument = ReturnDocument.After });
                return setResult["sequence_value"].AsInt32;
            }
        }
        
        return result["sequence_value"].AsInt32;
    }
    
    private async Task<int> GetMaxIdForSequence(string sequenceName)
    {
        // Determine which collection to query based on sequence name
        var collectionName = sequenceName switch
        {
            "media_id" => "medias",
            "episode_id" => "episodes",
            "user_id" => "users",
            _ => throw new ArgumentException($"Unknown sequence name: {sequenceName}")
        };
        
        var collection = database.GetCollection<BsonDocument>(collectionName);
        var sort = Builders<BsonDocument>.Sort.Descending("_id");
        var maxDoc = await collection.Find(new BsonDocument()).Sort(sort).Limit(1).FirstOrDefaultAsync();
        
        return maxDoc?["_id"].AsInt32 ?? 0;
    }

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

        var media = Builders<MongoMedia>
            .Update.Set("name", updatedMedia.Name)
            .Set("type", updatedMedia.Type)
            .Set("runtime", updatedMedia.Runtime)
            .Set("description", updatedMedia.Description)
            .Set("cover", updatedMedia.Cover)
            .Set("ageLimit", updatedMedia.AgeLimit)
            .Set("release", updatedMedia.Release.ToString("yyyy-MM-dd"))
            .Set("genres", updatedMedia.Genres.ToList());

        var options = new FindOneAndUpdateOptions<MongoMedia>
        {
            ReturnDocument = ReturnDocument.After,
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
            Id = await GetNextSequenceValue("media_id"),
            Name = newMedia.Name,
            Type = newMedia.Type,
            Runtime = newMedia.Runtime,
            Description = newMedia.Description,
            Cover = newMedia.Cover,
            AgeLimit = newMedia.AgeLimit,
            Release = newMedia.Release.ToString("yyyy-MM-dd"),
            Genres = newMedia.Genres.ToList(),
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
            throw new NotFoundException($"No episodes found for media with ID: {id}");
        }

        return result.Select(e => e.FromMongoEntityToDto()).ToList();
    }

    public async Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId)
    {
        var mediaCollection = database.GetCollection<MongoMedia>("medias");

        // Check if media exists AND contains the episode ID
        var mediaFilter = Builders<MongoMedia>.Filter.And(
            Builders<MongoMedia>.Filter.Eq(m => m.Id, id),
            Builders<MongoMedia>.Filter.AnyEq(m => m.Episodes, episodeId)
        );

        var mediaExists = await mediaCollection.Find(mediaFilter).AnyAsync();

        if (!mediaExists)
        {
            throw new NotFoundException(
                $"Media with ID {id} does not contain episode with ID {episodeId}"
            );
        }

        var episodeCollection = database.GetCollection<MongoEpisode>("episodes");

        var episodeFilter = Builders<MongoEpisode>.Filter.Eq(e => e.Id, episodeId);

        var result = await episodeCollection.Find(episodeFilter).SingleOrDefaultAsync();

        if (result is null)
        {
            throw new NotFoundException($"Episode with ID {episodeId} not found");
        }

        return result.FromMongoEntityToDto();
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        var userCollection = database.GetCollection<MongoUser>("users");
        var filter = Builders<MongoUser>.Filter.Empty;
        var results = await userCollection.Find(filter).ToListAsync();

        return results?.Select(doc => doc.FromMongoEntityToDto()).ToList() ?? [];
    }

    public async Task<UserDto> GetUserById(int id)
    {
        var userCollection = database.GetCollection<MongoUser>("users");
        var filter = Builders<MongoUser>.Filter.Eq(u => u.Id, id);
        var result = await userCollection.Find(filter).SingleOrDefaultAsync();

        if (result == null)
        {
            throw new NotFoundException("User not found");
        }

        return result.FromMongoEntityToDto();
    }

    public async Task AddMediaToWatchList(int userId, int profileId, int mediaId)
    {
        var userCollection = database.GetCollection<MongoUser>("users");
        var mediaCollection = database.GetCollection<MongoMedia>("medias");

        // Fetch user and media in parallel to reduce latency
        var userFilter = Builders<MongoUser>.Filter.Eq(u => u.Id, userId);
        var mediaFilter = Builders<MongoMedia>.Filter.Eq(m => m.Id, mediaId);

        var userTask = userCollection.Find(userFilter).SingleOrDefaultAsync();
        var mediaTask = mediaCollection.Find(mediaFilter).SingleOrDefaultAsync();

        await Task.WhenAll(userTask, mediaTask);

        var user = userTask.Result;
        var media = mediaTask.Result;

        // Validate user exists
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        // Validate profile exists (profileId is 0-based array index)
        if (profileId < 0 || profileId >= user.Profiles.Count)
        {
            throw new NotFoundException(
                $"Profile with index {profileId} not found for user {userId}"
            );
        }

        // Validate media exists
        if (media == null)
        {
            throw new NotFoundException($"Media with ID {mediaId} not found");
        }

        var profile = user.Profiles[profileId];

        // Check if media is already in watchlist
        if (profile.Watchlist.Medias.Contains(mediaId))
        {
            throw new BadRequestException("Media already in watchlist");
        }

        // Validate age restriction for child profiles
        if (profile.IsChild && media.AgeLimit.HasValue && media.AgeLimit.Value >= 18)
        {
            throw new BadRequestException(
                $"Content not appropriate for child profile (Age limit: {media.AgeLimit})"
            );
        }

        // Add media to watchlist using AddToSet (prevents duplicates at DB level)
        var update = Builders<MongoUser>.Update.AddToSet(
            $"profiles.{profileId}.watchlist.medias",
            mediaId
        );

        await userCollection.UpdateOneAsync(userFilter, update);
    }
}
