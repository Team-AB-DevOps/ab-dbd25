using api.Interfaces;
using api.Models.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    public IAuthService AuthService { get; set; }
    
    public AuthController(IAuthService authService)
    {
        AuthService = authService;
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var dto = await AuthService.Register(registerRequestDto);
        return CreatedAtAction(nameof(Register), dto);
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var tokenDto = await AuthService.Login(loginRequestDto);
        return Ok(tokenDto);
    }
    
    
}