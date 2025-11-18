using api.Models.DTOs.Domain;
using api.Repositories;
using api.Repositories.Interfaces;
using api.Services.Interfaces;

namespace api.Services;

public class UserService(IRepositoryFactory repositoryFactory) : IUserService
{
    public async Task<List<UserDto>> GetAllUsers(string tenant)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetAllUsers();
    }

    public async Task<UserDto> GetUserById(string tenant, int id)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetUserById(id);
    }

    public async Task<UserDto> AddMediaToWatchList(
        string tenant,
        int userId,
        int profileId,
        int mediaId
    )
    {
        var repository = repositoryFactory.GetRepository(tenant);

        await repository.AddMediaToWatchList(userId, profileId, mediaId);

        return await repository.GetUserById(userId);
    }
}
