using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MigrationTool.Models;

public class MongoReview
{
    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("mediaId")]
    [BsonRepresentation(BsonType.Int32)]
    public int MediaId { get; set; }

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }
}

public class MongoWatchlist
{
    [BsonElement("isLocked")]
    public bool IsLocked { get; set; }

    [BsonElement("medias")]
    [BsonRepresentation(BsonType.Int32)]
    public List<int> Medias { get; set; } = new List<int>();
}

public class MongoProfile
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("isChild")]
    public bool IsChild { get; set; }

    [BsonElement("watchlist")]
    public MongoWatchlist Watchlist { get; set; } = new MongoWatchlist();

    [BsonElement("reviews")]
    public List<MongoReview> Reviews { get; set; } = new List<MongoReview>();
}

public class MongoUser
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("subscriptions")]
    [BsonRepresentation(BsonType.Int32)]
    public List<int> Subscriptions { get; set; } = new List<int>();

    [BsonElement("privileges")]
    public List<string> Privileges { get; set; } = new List<string>();

    [BsonElement("profiles")]
    public List<MongoProfile> Profiles { get; set; } = new List<MongoProfile>();
}
