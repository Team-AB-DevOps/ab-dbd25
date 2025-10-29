namespace api.Models.DTOs.Auth;

public record TokenUserResponseDto(string Token, UserResponseDto User);
