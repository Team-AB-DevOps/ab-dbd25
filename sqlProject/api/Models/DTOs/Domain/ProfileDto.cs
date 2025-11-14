namespace api.Models.DTOs.Domain;

public record ProfileDto(
    string Name,
    bool? IsChild,
    WatchListDto WatchList,
    List<ReviewDto> Reviews
);
