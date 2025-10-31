using api.DTOs;
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
}