namespace api.DTOs;

public record ProfileDto(
    string Name,
    bool? IsChild,
    WatchListDto WatchList,
    List<ReviewDto> Reviews
);


    
    
    