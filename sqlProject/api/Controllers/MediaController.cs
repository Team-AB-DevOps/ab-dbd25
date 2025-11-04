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

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<MediaDto>> GetMediaById(int id)
    {
        var tenant = TenantHelper.GetTenant(Request);

        var media = await mediaService.GetMediaById(tenant, id);

        return Ok(media);
    }

    [Route("{id}/episodes")]
    [HttpGet]
    public async Task<ActionResult<List<EpisodeDto>>> GetAllMediaEpisodes(int id)
    {
        var tenant = TenantHelper.GetTenant(Request);

        var media = await mediaService.GetAllMediaEpisodes(tenant, id);

        return Ok(media);
    }

    [Route("{id}/episodes/{episodeId}")]
    [HttpGet]
    public async Task<ActionResult<EpisodeDto>> GetMediaEpisodeById(int id, int episodeId)
    {
        var tenant = TenantHelper.GetTenant(Request);

        var media = await mediaService.GetMediaEpisodeById(tenant, id, episodeId);

        return Ok(media);
    }
}