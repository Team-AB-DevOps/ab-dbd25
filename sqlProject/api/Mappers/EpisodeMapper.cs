using api.DTOs;
using api.Models;

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
}