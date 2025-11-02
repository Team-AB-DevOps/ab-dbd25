using api.DTOs;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    public IUserService UserService { get; set; }
    
    public UserController(IUserService userService)
    {
        UserService = userService;
    }


    [Route("")]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var tenant = TenantHelper.GetTenant(Request);
        
        var users = await UserService.GetAllUsers(tenant);
        
        return Ok(users);
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var tenant = TenantHelper.GetTenant(Request);
        var user = await UserService.GetUserById(tenant, id);

        return Ok(user);
    }
}


