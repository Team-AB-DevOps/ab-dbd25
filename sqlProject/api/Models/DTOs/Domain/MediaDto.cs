using System.Text.Json.Serialization;

namespace api.DTOs;

public record MediaDto(
    int Id,
    string Name,
    string Type,
    int Runtime,
    string Description,
    string Cover,
    int? AgeLimit,
    DateOnly Release,
    string[] Genres,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int[]? Episodes,
    MediaCreditsDto[] Credits
);

public record MediaCreditsDto(int PersonId, string[] Roles);

public record MediaIdDto(int MediaId);
