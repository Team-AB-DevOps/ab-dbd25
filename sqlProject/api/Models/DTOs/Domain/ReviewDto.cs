namespace api.Models.DTOs.Domain;

public record ReviewDto(int Id, int MediaId, int Rating, string? Description);
