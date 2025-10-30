using api.DTOs;
using api.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Repositories;

public class MongoRepository(IMongoDatabase database) : IRepository
{
    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var collection = database.GetCollection<BsonDocument>("medias");
        var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();

        return documents.Select(doc => doc.FromMongoEntityToDto()).ToList();
    }
}
