using api.Models.DTOs.Domain;

namespace api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers(string tenant);
    Task<UserDto> GetUserById(string tenant, int id);
    Task<UserDto> AddMediaToWatchList(string tenant, int userId, int profileId, int mediaId);
}
