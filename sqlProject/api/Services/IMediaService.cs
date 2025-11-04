using api.DTOs;

namespace api.Services;

public interface IMediaService
{
    Task<List<MediaDto>> GetAllMedias(string tenant);
    Task<MediaDto> GetMediaById(string tenant, int id);
    Task<List<EpisodeDto>> GetAllMediaEpisodes(string tenant, int id);
    Task<EpisodeDto> GetMediaEpisodeById(string tenant, int id, int episodeId);
}
