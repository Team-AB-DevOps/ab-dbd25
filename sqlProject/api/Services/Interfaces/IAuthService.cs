using api.Models.DTOs.Auth;

namespace api.Interfaces;

public interface IAuthService
{
    Task<UserResponseDto> Register(RegisterRequestDto dto);
    Task<TokenUserResponseDto> Login(LoginRequestDto dto);
}
