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

    public static MediaDto FromNeo4jRecordToDto(this IRecord record)
    {
        // Get the media node
        var mediaNode = record["m"].As<INode>();

        // Get collected lists
        var genresList = record["genres"].As<List<object>>();
        var episodesList = record["episodes"].As<List<object>>();
        var peopleList = record["people"].As<List<object>>();

        // Extract genre names
        var genres = genresList.Select(g => ((INode)g).Properties["name"].As<string>()).ToArray();

        // Extract episode IDs
        var episodes = episodesList
            .Select(e => (INode)e)
            .Where(e => e.Properties.ContainsKey("id"))
            .Select(e => e.Properties["id"].As<int>())
            .ToArray();

        // Extract person credits with roles
        var credits = peopleList
            .Select(p => p.As<Dictionary<string, object>>())
            .Where(dict => dict.ContainsKey("person") && dict["person"] != null)
            .Select(dict =>
            {
                var personNode = ((INode)dict["person"]);
                var role = dict.ContainsKey("role") ? dict["role"].As<string>() : "Actor";
                return new MediaCreditsDto(personNode.Properties["id"].As<int>(), new[] { role });
            })
            .ToArray();

        return new MediaDto(
            mediaNode.Properties.ContainsKey("id") ? mediaNode.Properties["id"].As<int>() : 0,
            mediaNode.Properties.ContainsKey("name")
                ? mediaNode.Properties["name"].As<string>()
                : string.Empty,
            mediaNode.Properties.ContainsKey("type")
                ? mediaNode.Properties["type"].As<string>()
                : string.Empty,
            mediaNode.Properties.ContainsKey("runtime")
                ? mediaNode.Properties["runtime"].As<int>()
                : 0,
            mediaNode.Properties.ContainsKey("description")
                ? mediaNode.Properties["description"].As<string>()
                : string.Empty,
            mediaNode.Properties.ContainsKey("cover")
                ? mediaNode.Properties["cover"].As<string>()
                : string.Empty,
            mediaNode.Properties.ContainsKey("ageLimit")
                ? mediaNode.Properties["ageLimit"].As<int>()
                : null,
            mediaNode.Properties.ContainsKey("release")
                ? DateOnly.Parse(mediaNode.Properties["release"].As<string>())
                : DateOnly.MinValue,
            genres,
            episodes.Any() ? episodes : null,
            credits
        );
    }
}
