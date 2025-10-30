namespace api.DTOs;

public record MediaDTO(
    int Id,
    string Name,
    string Type,
    int Runtime,
    string Description,
    string Cover,
    int? AgeLimit,
    DateOnly Release
);
