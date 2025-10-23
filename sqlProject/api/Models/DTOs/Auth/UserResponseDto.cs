namespace api.Models.DTOs.Auth;

public record UserResponseDto(string FirstName, string LastName, string Email, int Id);