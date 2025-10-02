using api.Models;

namespace api.Repositories;

public interface IRepository
{
    Task<List<Media>> GetAllMedias();
}