using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("/api")]
public class MediaController(IMediaService mediaService, IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private readonly string _tenant = httpContextAccessor.HttpContext?.Request.Headers["X-tenant"] ?? "sql";

    [Route("/medias")]
    [HttpGet]
    public async Task<ActionResult<List<MediaDTO>>> GetAllMedias()
    {
        var medias = await mediaService.GetAllMedias(_tenant);

        if (medias.Count == 0)
        {
            return NoContent();
        }

        return Ok(medias);
    }
}