using api.DTOs;
using MongoDB.Bson;
using Neo4j.Driver;

namespace api.Mappers;

public static class MediaMapper
{
    public static MediaDTO FromMongoEntityToDto(this BsonDocument mongoEntity)
    {
        return new MediaDTO(
            mongoEntity["_id"].AsInt32,
            mongoEntity["name"].AsString,
            mongoEntity["type"].AsString,
            mongoEntity["runtime"].AsInt32,
            mongoEntity["description"].AsString,
            mongoEntity["cover"].AsString,
            mongoEntity["ageLimit"].AsInt32,
            DateOnly.Parse(mongoEntity["release"].AsString)
        );
    }

    public static MediaDTO FromNeo4JEntityToDto(this INode nodeEntity)
    {
        return new MediaDTO(
            nodeEntity.Properties["id"].As<int>(),
            nodeEntity.Properties["name"].As<string>(),
            nodeEntity.Properties["type"].As<string>(),
            nodeEntity.Properties["runtime"].As<int>(),
            nodeEntity.Properties["description"].As<string>(),
            nodeEntity.Properties["cover"].As<string>(),
            nodeEntity.Properties["ageLimit"].As<int>(),
            DateOnly.Parse(nodeEntity.Properties["release"].As<string>())
        );
    }
}
