using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models.Mongo;

public class MongoPerson
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("gender")]
    public string Gender { get; set; }

    [BsonElement("birthDate")]
    public string BirthDate { get; set; }
}
