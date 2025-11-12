using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class MediaMapper
{
    public static MediaDto FromSqlEntityToDto(this Media media)
    {
        return new MediaDto(
            media.Id,
            media.Name,
            media.Type,
            media.Runtime,
            media.Description,
            media.Cover,
            media.AgeLimit,
            media.Release,
            media.Genres.Select(g => g.Name).ToArray(),
            media.Episodes?.Count > 0 ? media.Episodes.Select(e => e.Id).ToArray() : null,
            media
                .MediaPersonRoles.GroupBy(x => x.PersonId)
                .Select(group => new MediaCreditsDto(
                    group.Key,
                    group.Select(x => x.Role.Name).ToArray()
                ))
                .ToArray()
        );
    }

    // public static MediaDto FromMongoEntityToDto(this BsonDocument mongoEntity)
    // {
    //     return new MediaDto(
    //         mongoEntity["_id"].AsInt32,
    //         mongoEntity["name"].AsString,
    //         mongoEntity["type"].AsString,
    //         mongoEntity["runtime"].AsInt32,
    //         mongoEntity["description"].AsString,
    //         mongoEntity["cover"].AsString,
    //         mongoEntity["ageLimit"].AsInt32,
    //         DateOnly.Parse(mongoEntity["release"].AsString)
    //     );
    // }
    //
    // public static MediaDto FromNeo4JEntityToDto(this INode nodeEntity)
    // {
    //     return new MediaDto(
    //         nodeEntity.Properties["id"].As<int>(),
    //         nodeEntity.Properties["name"].As<string>(),
    //         nodeEntity.Properties["type"].As<string>(),
    //         nodeEntity.Properties["runtime"].As<int>(),
    //         nodeEntity.Properties["description"].As<string>(),
    //         nodeEntity.Properties["cover"].As<string>(),
    //         nodeEntity.Properties["ageLimit"].As<int>(),
    //         DateOnly.Parse(nodeEntity.Properties["release"].As<string>())
    //     );
    // }
}