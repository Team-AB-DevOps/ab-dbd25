namespace api.DTOs;

public record EpisodeDto(
    int Id,
    string Name,
    int SeasonCount,
    int EpisodeCount,
    int RunTime,
    string Description,
    DateOnly Release
);
