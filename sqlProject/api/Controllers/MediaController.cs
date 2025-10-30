using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("/api")]
public class MediaController(IMediaService mediaService) : ControllerBase
{
    [Route("/medias")]
    [HttpGet]
    public async Task<ActionResult<List<MediaDTO>>> GetAllMedias()
    {
        var tenant = GetTenant();

        var medias = await mediaService.GetAllMedias(tenant);

        if (medias.Count == 0)
        {
            return NoContent();
        }

        return Ok(medias);
    }

    private string GetTenant()
    {
        var tenant = Request.Headers["X-tenant"].ToString();
        return string.IsNullOrWhiteSpace(tenant) ? "sql" : tenant;
    }
}
