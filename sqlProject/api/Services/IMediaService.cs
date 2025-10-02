using api.DTOs;
using api.Models;

namespace api.Services;

public interface IMediaService
{
    Task<List<MediaDTO>> GetAllMedias();
}