using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Models.Sql;
using Neo4j.Driver;

namespace api.Mappers;

public static class EpisodeMapper
{
    public static EpisodeDto FromSqlEntityToDto(this Episode episode)
    {
        return new EpisodeDto(
            episode.Id,
            episode.Name,
            episode.SeasonCount ?? 1,
            episode.EpisodeCount,
            episode.Runtime,
            episode.Description,
            episode.Release
        );
    }

    public static EpisodeDto FromMongoEntityToDto(this MongoEpisode mongoEntity)
    {
        return new EpisodeDto(
            mongoEntity.Id,
            mongoEntity.Name,
            mongoEntity.SeasonCount,
            mongoEntity.EpisodeCount,
            mongoEntity.Runtime,
            mongoEntity.Description,
            DateOnly.Parse(mongoEntity.Release)
        );
    }

    public static EpisodeDto FromNeo4jRecordToEpisodeDto(this IRecord record)
    {
        if (record["e"] == null)
        {
            throw new ArgumentNullException(
                nameof(record),
                "Episode node (record[\"e\"]) cannot be null"
            );
        }
        var episodeNode = record["e"].As<INode>();

        return new EpisodeDto(
            episodeNode.Properties.ContainsKey("id") ? episodeNode.Properties["id"].As<int>() : 0,
            episodeNode.Properties.ContainsKey("name")
                ? episodeNode.Properties["name"].As<string>()
                : string.Empty,
            episodeNode.Properties.ContainsKey("seasonCount")
                ? episodeNode.Properties["seasonCount"].As<int>()
                : 1,
            episodeNode.Properties.ContainsKey("episodeCount")
                ? episodeNode.Properties["episodeCount"].As<int>()
                : 0,
            episodeNode.Properties.ContainsKey("runtime")
                ? episodeNode.Properties["runtime"].As<int>()
                : 0,
            episodeNode.Properties.ContainsKey("description")
                ? episodeNode.Properties["description"].As<string>()
                : string.Empty,
            episodeNode.Properties.ContainsKey("release")
                ? DateOnly.Parse(episodeNode.Properties["release"].As<string>())
                : DateOnly.MinValue
        );
    }
}
