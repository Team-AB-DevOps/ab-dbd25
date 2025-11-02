using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class ProfileMapper
{
    public static ProfileDto FromSqlEntityToDto(this Profile profile)
    {
        return new ProfileDto(
            profile.Name,
            profile.IsChild,
            new WatchListDto(
                    profile.WatchList.IsLocked,
                    profile.WatchList.Medias?.Select(m => m.Id).ToList()
                ),
            profile.Reviews?.Select((r, index) => new ReviewDto(
                index,
                r.MediaId,
                r.Rating,
                r.Description
            )).ToList()
        );
    }
}
