using api.DTOs;

namespace api.Repositories;

public interface IRepository
{
    Task<List<MediaDto>> GetAllMedias();
    Task<MediaDto> GetMediaById(int id);
    Task DeleteMediaById(int id);
    Task<List<EpisodeDto>> GetAllMediaEpisodes(int id);
    Task<EpisodeDto> GetMediaEpisodeById(int id, int episodeId);
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int id);
}
