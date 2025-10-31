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
    public async Task<ActionResult<List<MediaDto>>> GetAllMedias()
    {
        var tenant = GetTenant();

        var medias = await mediaService.GetAllMedias(tenant);

        if (medias.Count == 0)
        {
            return NoContent();
        }

        return Ok(medias);
    }

    [Route("/medias/{id}")]
    [HttpGet]
    public async Task<ActionResult<List<MediaDto>>> GetAllMedias(int id)
    {
        var tenant = GetTenant();

        var media = await mediaService.GetMediaById(tenant, id);

        return Ok(media);
    }

    private string GetTenant()
    {
        var tenant = Request.Headers["X-tenant"].ToString();
        return string.IsNullOrWhiteSpace(tenant) ? "sql" : tenant;
    }
}