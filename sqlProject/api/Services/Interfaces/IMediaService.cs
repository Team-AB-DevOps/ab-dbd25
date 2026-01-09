using api.Models.DTOs.Domain;

namespace api.Services.Interfaces;

public interface IMediaService
{
    Task<List<MediaDto>> GetAllMedias(string tenant);
    Task<MediaDto> GetMediaById(string tenant, int id);
    Task<List<MediaDto>> SearchMediasByName(string tenant, string name);
    Task<MediaDto> UpdateMedia(string tenant, UpdateMediaDto updatedMedia, int id);
    Task<MediaDto> CreateMedia(string tenant, CreateMediaDto newMedia);
    Task DeleteMediaById(string tenant, int id);
    Task<List<EpisodeDto>> GetAllMediaEpisodes(string tenant, int id);
    Task<EpisodeDto> GetMediaEpisodeById(string tenant, int id, int episodeId);
}