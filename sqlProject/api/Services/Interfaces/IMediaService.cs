using api.DTOs;

namespace api.Services;

public interface IMediaService
{
    Task<List<MediaDto>> GetAllMedias(string tenant);
}
