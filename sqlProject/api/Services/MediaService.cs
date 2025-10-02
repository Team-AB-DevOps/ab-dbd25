using api.Data;
using api.Models;
using api.DTOs;
using api.DTOs.Mappers;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class MediaService(DataContext context) : IMediaService
{
    private readonly DataContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var medias = await _context.Medias.ToListAsync();
        var mapper = new MediaMapper();
        var mediaDtos = medias.Select(mapper.MediaToMediaDto).ToList();
        
        return mediaDtos;
    }
}