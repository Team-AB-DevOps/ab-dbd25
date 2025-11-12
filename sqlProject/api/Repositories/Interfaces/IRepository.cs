using api.DTOs;
using api.Models.DTOs.Domain;

namespace api.Repositories;

public interface IRepository
{
    Task<List<MediaDto>> GetAllMedias();
    Task<MediaDto> GetMediaById(int id);
    Task<MediaDto> UpdateMedia(UpdateMediaDto updatedMedia, int id);
    Task<MediaDto> CreateMedia(CreateMediaDto newMedia);
    Task DeleteMediaById(int id);
    Task<List<EpisodeDto>> GetAllMediaEpisodes(int id);
    Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId);
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int id);
    Task AddMediaToWatchList(int userId, int profileId, int mediaId);
}
