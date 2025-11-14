using api.Models.DTOs.Auth;

namespace api.Services.Interfaces;

public interface IAuthService
{
    Task<UserResponseDto> Register(RegisterRequestDto dto);
    Task<TokenUserResponseDto> Login(LoginRequestDto dto);
}
