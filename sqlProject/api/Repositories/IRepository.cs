using api.DTOs;

namespace api.Repositories;

public interface IRepository
{
    Task<List<MediaDTO>> GetAllMedias();
}
