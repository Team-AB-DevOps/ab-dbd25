namespace api.Models.DTOs.Domain;

public record CreateMediaDto(
    string Name,
    string Type,
    int Runtime,
    string Description,
    string Cover,
    int? AgeLimit,
    DateOnly Release,
    string[] Genres
);
