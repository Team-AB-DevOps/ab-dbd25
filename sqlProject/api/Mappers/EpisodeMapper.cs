using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Models.Sql;

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
}
