namespace api.Models.DTOs.Domain;

public record WatchListDto(bool? IsLocked, List<int> Medias);
