using api.DTOs;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.Repositories;

public class MongoRepository : IRepository
{
    private readonly IMongoDatabase _database;

    public MongoRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MongoConnectionString"];
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(mongoUrl);
        _database = client.GetDatabase(mongoUrl.DatabaseName);
    }

    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var collection = _database.GetCollection<BsonDocument>("medias");
        var documents = await collection.Find(new BsonDocument()).ToListAsync();

        // Map BsonDocument to MediaDTO
        return documents.Select(doc => new MediaDTO
        (
            doc["_id"].AsInt32,
            doc["name"].AsString,
            doc["type"].AsString,
            doc["runtime"].AsInt32,
            doc["description"].AsString,
            doc["cover"].AsString,
            doc["ageLimit"].AsInt32,
            DateOnly.Parse(doc["release"].AsString)
        )).ToList();
    }
}