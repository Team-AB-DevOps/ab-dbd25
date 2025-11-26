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

        // Group records by media ID to aggregate related data
        // Use ElementId as fallback for nodes without 'id' property
        var mediaGroups = records.GroupBy(r =>
        {
            var node = r["m"].As<INode>();
            return node.Properties.ContainsKey("id") 
                ? node.Properties["id"].As<int>().ToString() 
                : node.ElementId;
        });

        return mediaGroups.Select(group =>
        {
            var recordsList = group.ToList();

            // Aggregate episodes, genres, and persons for this media (filtering out nulls from OPTIONAL MATCH)
            var episodesList = recordsList
                .Select(r => r["e"])
                .Where(node => node != null && node.GetType() != typeof(object))
                .Select(node => node.As<INode>())
                .ToList();
            
            var genresList = recordsList
                .Select(r => r["g"])
                .Where(node => node != null && node.GetType() != typeof(object))
                .Select(node => node.As<INode>())
                .ToList();
            
            var personsList = recordsList
                .Select(r => r["p"])
                .Where(node => node != null && node.GetType() != typeof(object))
                .Select(node => node.As<INode>())
                .ToList();

            // Convert to dictionary format
            var recordDict = new Dictionary<string, object>
            {
                ["m"] = recordsList.First()["m"].As<INode>(),
                ["episodes"] = episodesList,
                ["genres"] = genresList,
                ["persons"] = personsList
            };

            // Convert to MediaDto using the single-record mapper
            return recordDict.FromNeo4jRecordToDto();
        }).ToList();
    }

    private static MediaDto FromNeo4jRecordToDto(this Dictionary<string, object> record)
    {
        // Get nodes from the record
        var mediaNode = (INode)record["m"];
        var episodeNodes = (List<INode>)record["episodes"];
        var genreNodes = (List<INode>)record["genres"];
        var personNodes = (List<INode>)record["persons"];

        // Extract unique episode IDs (only for episodes that have an id)
        var episodes = episodeNodes
            .Where(e => e.Properties.ContainsKey("id"))
            .DistinctBy(e => e.Properties["id"].As<int>())
            .Select(e => e.Properties["id"].As<int>())
            .ToArray();

        // Extract unique genre names
        var genres = genreNodes
            .DistinctBy(g => g.Properties["name"].As<string>())
            .Select(g => g.Properties["name"].As<string>())
            .ToArray();

        // Extract unique person credits (only for persons that have an id)
        var credits = personNodes
            .Where(p => p.Properties.ContainsKey("id"))
            .DistinctBy(p => p.Properties["id"].As<int>())
            .Select(p => new MediaCreditsDto(
                p.Properties["id"].As<int>(),
                new[] { "Actor" } // Role information not in current query
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
