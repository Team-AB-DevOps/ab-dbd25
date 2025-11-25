using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Models.Sql;

namespace api.Mappers;

public static class UserMapper
{
    public static UserDto FromSqlEntityToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Subscriptions.Select(s => s.Id).ToList(),
            user.Privileges.Select(p => p.Name).ToList(),
            user.Profiles.Select(profile => profile.FromSqlEntityToDto()).ToList()
        );
    }

    public static UserDto FromMongoEntityToDto(this MongoUser mongoEntity)
    {
        return new UserDto(
            mongoEntity.Id,
            mongoEntity.FirstName,
            mongoEntity.LastName,
            mongoEntity.Subscriptions.ToList(),
            mongoEntity.Privileges.ToList(),
            mongoEntity.Profiles.Select(p => new ProfileDto(
                p.Name,
                p.IsChild,
                new WatchListDto(p.Watchlist.IsLocked, p.Watchlist.Medias.ToList()),
                p.Reviews.Select(r => new ReviewDto(r.Id, r.MediaId, r.Rating, r.Description)).ToList()
            )).ToList()
        );
    }
}