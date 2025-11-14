using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models.Mongo;

public class MongoSubscription
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("price")]
    public int Price { get; set; }
}
