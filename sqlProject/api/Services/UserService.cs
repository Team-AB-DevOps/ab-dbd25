using api.DTOs;
using api.Interfaces;
using api.Repositories;

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
}
