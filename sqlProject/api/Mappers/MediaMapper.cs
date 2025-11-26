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

    public static List<MediaDto> FromNeo4jRecordsToDto(this List<IRecord> records)
    {
        if (!records.Any())
            return new List<MediaDto>();

        // With collect, each record is already aggregated per media
        return records.Select(record => record.FromNeo4jRecordToDto()).ToList();
    }

    private static MediaDto FromNeo4jRecordToDto(this IRecord record)
    {
        // Get the media node
        var mediaNode = record["m"].As<INode>();
        
        // Get collected lists (these are already aggregated by the query)
        var genresList = record["genres"].As<List<object>>();
        var episodesList = record["episodes"].As<List<object>>();
        var peopleList = record["people"].As<List<object>>();

        // Extract unique genre names (filter out null entries from OPTIONAL MATCH)
        var genres = genresList
            .Where(g => g != null && g.GetType() != typeof(object))
            .Select(g => ((INode)g).Properties["name"].As<string>())
            .Distinct()
            .ToArray();

        // Extract unique episode IDs (filter out null entries)
        var episodes = episodesList
            .Where(e => e != null && e.GetType() != typeof(object))
            .Select(e => (INode)e)
            .Where(e => e.Properties.ContainsKey("id"))
            .Select(e => e.Properties["id"].As<int>())
            .Distinct()
            .ToArray();

        // Extract person credits with roles
        var credits = peopleList
            .Where(p => p != null && p.GetType() != typeof(object))
            .Select(p => p.As<Dictionary<string, object>>())
            .Where(dict => dict.ContainsKey("person") && dict["person"] != null)
            .Select(dict =>
            {
                var personNode = ((INode)dict["person"]);
                var role = dict.ContainsKey("role") ? dict["role"].As<string>() : "Actor";
                return new { PersonNode = personNode, Role = role };
            })
            .Where(x => x.PersonNode.Properties.ContainsKey("id"))
            .GroupBy(x => x.PersonNode.Properties["id"].As<int>())
            .Select(group => new MediaCreditsDto(
                group.Key,
                group.Select(x => x.Role).Distinct().ToArray()
            ))
            .ToArray();

        return new MediaDto(
            mediaNode.Properties.ContainsKey("id") ? mediaNode.Properties["id"].As<int>() : 0,
            mediaNode.Properties.ContainsKey("name") ? mediaNode.Properties["name"].As<string>() : string.Empty,
            mediaNode.Properties.ContainsKey("type") ? mediaNode.Properties["type"].As<string>() : string.Empty,
            mediaNode.Properties.ContainsKey("runtime") ? mediaNode.Properties["runtime"].As<int>() : 0,
            mediaNode.Properties.ContainsKey("description") ? mediaNode.Properties["description"].As<string>() : string.Empty,
            mediaNode.Properties.ContainsKey("cover") ? mediaNode.Properties["cover"].As<string>() : string.Empty,
            mediaNode.Properties.ContainsKey("ageLimit") ? mediaNode.Properties["ageLimit"].As<int>() : null,
            mediaNode.Properties.ContainsKey("release") ? DateOnly.Parse(mediaNode.Properties["release"].As<string>()) : DateOnly.MinValue,
            genres,
            episodes.Any() ? episodes : null,
            credits
        );
    }
}
