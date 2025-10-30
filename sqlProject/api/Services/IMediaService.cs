using api.DTOs;

namespace api.Services;

public interface IMediaService
{
    Task<List<MediaDTO>> GetAllMedias(string tenant);
}