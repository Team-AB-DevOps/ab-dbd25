namespace api.DTOs;

public record WatchListDto(
    bool? IsLocked,
    List<int> Medias
);