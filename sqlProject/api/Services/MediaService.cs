﻿using api.DTOs;
using api.Repositories;

namespace api.Services;

public class MediaService(IRepositoryFactory repositoryFactory) : IMediaService
{
    public async Task<List<MediaDto>> GetAllMedias(string tenant)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetAllMedias();
    }

    public async Task<MediaDto> GetMediaById(string tenant, int id)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetMediaById(id);
    }

    public async Task<List<EpisodeDto>> GetAllMediaEpisodes(string tenant, int id)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetAllMediaEpisodes(id);
    }

    public async Task<EpisodeDto> GetMediaEpisodeById(string tenant, int id, int episodeId)
    {
        var repository = repositoryFactory.GetRepository(tenant);

        return await repository.GetMediaEpisodeById(id, episodeId);
    }
}