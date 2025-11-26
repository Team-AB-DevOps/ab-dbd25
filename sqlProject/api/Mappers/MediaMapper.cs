using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Models.Sql;
using Neo4j.Driver;

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

    public static MediaDto FromMongoEntityToDto(this MongoMedia mongoEntity)
    {
        return new MediaDto(
            mongoEntity.Id,
            mongoEntity.Name,
            mongoEntity.Type,
            mongoEntity.Runtime,
            mongoEntity.Description,
            mongoEntity.Cover,
            mongoEntity.AgeLimit,
            DateOnly.Parse(mongoEntity.Release),
            mongoEntity.Genres?.ToArray() ?? [],
            mongoEntity.Episodes?.ToArray() ?? null,
            mongoEntity
                .Credits?.Select(c => new MediaCreditsDto(c.PersonId, c.Roles?.ToArray() ?? []))
                .ToArray() ?? []
        );
    }

    public static MediaDto FromNeo4jRecordToDto(this Dictionary<string, object> record)
    {
        // Get nodes from the record
        var mediaNode = (INode)record["m"];
        var episodeNodes = (List<INode>)record["episodes"];
        var genreNodes = (List<INode>)record["genres"];
        var personNodes = (List<INode>)record["persons"];

        // Extract unique episode IDs
        var episodes = episodeNodes
            .DistinctBy(e => e.Properties["id"].As<int>())
            .Select(e => e.Properties["id"].As<int>())
            .ToArray();

        // Extract unique genre names
        var genres = genreNodes
            .DistinctBy(g => g.Properties["name"].As<string>())
            .Select(g => g.Properties["name"].As<string>())
            .ToArray();

        // Extract unique person credits
        var credits = personNodes
            .DistinctBy(p => p.Properties["id"].As<int>())
            .Select(p => new MediaCreditsDto(
                p.Properties["id"].As<int>(),
                new[] { "Actor" } // Role information not in current query
            ))
            .ToArray();

        return new MediaDto(
            mediaNode.Properties["id"].As<int>(),
            mediaNode.Properties["name"].As<string>(),
            mediaNode.Properties["type"].As<string>(),
            mediaNode.Properties["runtime"].As<int>(),
            mediaNode.Properties["description"].As<string>(),
            mediaNode.Properties["cover"].As<string>(),
            mediaNode.Properties.ContainsKey("ageLimit") ? mediaNode.Properties["ageLimit"].As<int>() : null,
            DateOnly.Parse(mediaNode.Properties["release"].As<string>()),
            genres,
            episodes.Any() ? episodes : null,
            credits
        );
    }
}
