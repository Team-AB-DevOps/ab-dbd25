using api.DTOs;
using api.Models;
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
        var medias  = await mediaService.GetAllMedias();

        if (medias.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(medias);
    }
}