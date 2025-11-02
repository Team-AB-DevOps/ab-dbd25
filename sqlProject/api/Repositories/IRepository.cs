using api.DTOs;

namespace api.Repositories;

public interface IRepository
{
    Task<List<MediaDto>> GetAllMedias();
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto> GetUserById(int id);
}
