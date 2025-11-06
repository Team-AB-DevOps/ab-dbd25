using api.DTOs;
using api.Models.DTOs.Domain;

namespace api.Services;

public interface IMediaService
{
    Task<List<MediaDto>> GetAllMedias(string tenant);
    Task<MediaDto> GetMediaById(string tenant, int id);
    Task<MediaDto> UpdateMedia(string tenant, MediaDto updatedMedia, int id);
    Task<MediaDto> CreateMedia(string tenant, CreateMediaDto newMedia);

    Task<List<EpisodeDto>> GetAllMediaEpisodes(string tenant, int id);
    Task<EpisodeDto> GetMediaEpisodeById(string tenant, int id, int episodeId);
}
