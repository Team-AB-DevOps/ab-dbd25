using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MigrationTool.Models;

public class MongoEpisode
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("seasonCount")]
    public int SeasonCount { get; set; }
    
    [BsonElement("episodeCount")]
    public int EpisodeCount { get; set; }
    
    [BsonElement("runtime")]
    public int Runtime { get; set; }
    
    [BsonElement("description")]
    public string Description { get; set; }
    
    [BsonElement("release")]
    public string Release { get; set; }
}
