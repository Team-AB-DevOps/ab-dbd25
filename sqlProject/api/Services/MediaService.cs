using api.DTOs;
using api.Repositories;

namespace api.Services;

public class MediaService(
    IRepositoryFactory repositoryFactory,
    IHttpContextAccessor httpContextAccessor) : IMediaService
{
    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var tenant = httpContextAccessor.HttpContext!.Request.Headers["X-tenant"];
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetAllMedias();
    }
}