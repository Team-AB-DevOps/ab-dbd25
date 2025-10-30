using api.DTOs;
using api.Repositories;

namespace api.Services;

public class MediaService(IRepositoryFactory repositoryFactory) : IMediaService
{
    public async Task<List<MediaDTO>> GetAllMedias(string tenant)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetAllMedias();
    }
}
