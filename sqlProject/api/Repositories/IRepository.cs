using api.DTOs;
using api.Models;

namespace api.Repositories;

public interface IRepository
{
    Task<List<MediaDTO>> GetAllMedias();
}