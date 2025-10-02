using api.DTOs;
using api.DTOs.Mappers;
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

        var medias = await repository.GetAllMedias();
        var mapper = new MediaMapper();

        return mapper.ToMediaDtos(medias);
    }
}