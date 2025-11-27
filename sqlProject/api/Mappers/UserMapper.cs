using api.Models.DTOs.Domain;
using api.Models.Mongo;
using api.Models.Sql;
using Neo4j.Driver;

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
            mongoEntity
                .Profiles.Select(p => new ProfileDto(
                    p.Name,
                    p.IsChild,
                    new WatchListDto(
                        p.Watchlist.IsLocked,
                        p.Watchlist.Medias.OrderBy(m => m).ToList()
                    ),
                    p.Reviews.Select(r => new ReviewDto(r.Id, r.MediaId, r.Rating, r.Description))
                        .ToList()
                ))
                .ToList()
        );
    }

    public static UserDto FromNeo4jRecordToUserDto(this IRecord record)
    {
        // Get the user node
        var userNode = record["u"].As<INode>();

        // Get collected lists (already deduplicated by DISTINCT in query)
        var subscriptionsList = record["subscriptions"].As<List<object>>();
        var privilegesList = record["privileges"].As<List<object>>();
        var profilesDataList = record["profilesData"].As<List<object>>();

        // Extract subscription IDs (filter out null entries from OPTIONAL MATCH)
        var subscriptions = subscriptionsList
            .Where(s => s != null && s.GetType() != typeof(object))
            .Select(s => (INode)s)
            .Where(s => s.Properties.ContainsKey("id"))
            .Select(s => s.Properties["id"].As<int>())
            .ToList();

        // Extract privilege names
        var privileges = privilegesList
            .Where(p => p != null && p.GetType() != typeof(object))
            .Select(p => (INode)p)
            .Where(p => p.Properties.ContainsKey("name"))
            .Select(p => p.Properties["name"].As<string>())
            .ToList();

        // Extract profiles with their associated reviews and watchlists
        var profiles = profilesDataList
            .Where(pd => pd != null && pd.GetType() != typeof(object))
            .Select(pd => pd.As<Dictionary<string, object>>())
            .Where(dict => dict.ContainsKey("profile") && dict["profile"] != null)
            .Select(
                (dict, profileIndex) =>
                {
                    var profileNode = (INode)dict["profile"];

                    // Extract reviews for this specific profile
                    var profileReviewsList =
                        dict.ContainsKey("reviews") && dict["reviews"] != null
                            ? dict["reviews"].As<List<object>>()
                            : new List<object>();

                    var reviews = profileReviewsList
                        .Where(r => r != null && r.GetType() != typeof(object))
                        .Select(r => r.As<Dictionary<string, object>>())
                        .Where(reviewDict =>
                            reviewDict.ContainsKey("media") && reviewDict["media"] != null
                        )
                        .Select(
                            (reviewDict, index) =>
                            {
                                var mediaNode = (INode)reviewDict["media"];

                                var rating =
                                    reviewDict.ContainsKey("rating") && reviewDict["rating"] != null
                                        ? reviewDict["rating"].As<int>()
                                        : 0;
                                var description =
                                    reviewDict.ContainsKey("description")
                                    && reviewDict["description"] != null
                                        ? reviewDict["description"].As<string>()
                                        : null;

                                return new ReviewDto(
                                    index + 1, // Generate review ID
                                    mediaNode.Properties["id"].As<int>(),
                                    rating,
                                    description
                                );
                            }
                        )
                        .ToList();

                    // Extract watchlists for this specific profile
                    var profileWatchlistsList =
                        dict.ContainsKey("watchlists") && dict["watchlists"] != null
                            ? dict["watchlists"].As<List<object>>()
                            : new List<object>();

                    var watchlistData = profileWatchlistsList
                        .Where(w => w != null && w.GetType() != typeof(object))
                        .Select(w => w.As<Dictionary<string, object>>())
                        .Where(wDict =>
                            wDict.ContainsKey("watchlist") && wDict["watchlist"] != null
                        )
                        .Select(wDict => new
                        {
                            Watchlist = (INode)wDict["watchlist"],
                            Media = wDict.ContainsKey("media") && wDict["media"] != null
                                ? (INode)wDict["media"]
                                : null,
                        })
                        .ToList();

                    // Build watchlist DTO
                    var watchlistMediaIds = watchlistData
                        .Where(w => w.Media != null && w.Media.Properties.ContainsKey("id"))
                        .Select(w => w.Media!.Properties["id"].As<int>())
                        .ToList();

                    var isLocked =
                        watchlistData.FirstOrDefault()?.Watchlist.Properties.ContainsKey("isLocked")
                        == true
                            ? watchlistData.First().Watchlist.Properties["isLocked"].As<bool?>()
                            : null;

                    var watchlist = new WatchListDto(isLocked, watchlistMediaIds);

                    return new ProfileDto(
                        profileNode.Properties.ContainsKey("name")
                            ? profileNode.Properties["name"].As<string>()
                            : string.Empty,
                        profileNode.Properties.ContainsKey("isChild")
                            ? profileNode.Properties["isChild"].As<bool?>()
                            : null,
                        watchlist,
                        reviews
                    );
                }
            )
            .ToList();

        return new UserDto(
            userNode.Properties.ContainsKey("id") ? userNode.Properties["id"].As<int>() : 0,
            userNode.Properties.ContainsKey("firstName")
                ? userNode.Properties["firstName"].As<string>()
                : string.Empty,
            userNode.Properties.ContainsKey("lastName")
                ? userNode.Properties["lastName"].As<string>()
                : string.Empty,
            subscriptions,
            privileges,
            profiles
        );
    }
}
