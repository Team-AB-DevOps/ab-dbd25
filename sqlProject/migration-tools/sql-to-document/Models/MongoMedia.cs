using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MigrationTool.Models;

public class MongoCredit
{
    [BsonElement("personId")]
    [BsonRepresentation(BsonType.Int32)]
    public int PersonId { get; set; }
    
    [BsonElement("roles")]
    public List<string> Roles { get; set; } = new List<string>();
}

public class MongoMedia
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("type")]
    public string Type { get; set; }
    
    [BsonElement("runtime")]
    public int Runtime { get; set; }
    
    [BsonElement("description")]
    public string Description { get; set; }
    
    [BsonElement("cover")]
    public string Cover { get; set; }
    
    [BsonElement("ageLimit")]
    public int? AgeLimit { get; set; }
    
    [BsonElement("release")]
    public string Release { get; set; }
    
    [BsonElement("genres")]
    public List<string> Genres { get; set; } = new List<string>();
    
    [BsonElement("episodes")]
    [BsonRepresentation(BsonType.Int32)]
    public List<int> Episodes { get; set; } = new List<int>();
    
    [BsonElement("credits")]
    public List<MongoCredit> Credits { get; set; } = new List<MongoCredit>();
}
