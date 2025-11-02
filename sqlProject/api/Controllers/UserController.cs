using api.DTOs;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "USER")]
    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        // Get the authenticated user's ID from JWT token claims
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authenticatedUserId))
        {
            return Unauthorized("Invalid token");
        }

        // Ensure the authenticated user can only access their own data
        if (authenticatedUserId != id)
        {
            return Forbid();
        }

        var tenant = TenantHelper.GetTenant(Request);
        var user = await UserService.GetUserById(tenant, id);

        return Ok(user);
    }
}
