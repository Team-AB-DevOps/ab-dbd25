using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MigrationTool.Models;

public class MongoAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("password")]
    public string Password { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.Int32)]
    public int UserId { get; set; }
}
