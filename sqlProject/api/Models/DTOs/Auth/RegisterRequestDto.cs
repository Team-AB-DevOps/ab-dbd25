namespace api.Models.DTOs.Auth;

public record RegisterRequestDto(string FirstName, string LastName, string Email, string Password, string? Password2);