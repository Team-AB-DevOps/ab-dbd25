using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("/api/medias")]
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [Route("")]
    [HttpGet]
    public async Task<ActionResult<List<MediaDto>>> GetAllMedias()
    {
        var tenant = TenantHelper.GetTenant(Request);
        
        var medias = await mediaService.GetAllMedias(tenant);

        return Ok(medias);
    }
    
}
