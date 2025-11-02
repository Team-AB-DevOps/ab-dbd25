namespace api.DTOs;

public record ReviewDto(int Id, int MediaId, int Rating, string? Description);
