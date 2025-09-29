using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MigrationTool.Services;

public class MongoService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoService> _logger;

    public MongoService(IConfiguration configuration, ILogger<MongoService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("MongoConnection");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("ab_database_mongo");
        
        _logger.LogInformation("Connected to MongoDB database: ab_database_mongo");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
            _logger.LogInformation("MongoDB connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDB connection test failed");
            return false;
        }
    }

    public async Task CreateIndexesAsync()
    {
        // Add any indexes you need for your collections
        _logger.LogInformation("Creating MongoDB indexes...");
        
        // Example: Create index on User collection
        var userCollection = GetCollection<dynamic>("users");
        var indexKeysDefinition = Builders<dynamic>.IndexKeys.Ascending("email");
        await userCollection.Indexes.CreateOneAsync(new CreateIndexModel<dynamic>(indexKeysDefinition));
        
        _logger.LogInformation("MongoDB indexes created successfully");
    }
}
