using api.Models;

namespace SqlToGraph.Models;

// =============================================
// NEO4J GRAPH NODE MODELS
// =============================================
// These models represent the target graph structure in Neo4j
// Source models are imported from api.Models

public class GraphUser
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class GraphProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsChild { get; set; }
}

public class GraphWatchList
{
    public int Id { get; set; }
    public bool IsLocked { get; set; }
}

public class GraphMedia
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Runtime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Cover { get; set; } = string.Empty;
    public int? AgeLimit { get; set; }
    public DateOnly Release { get; set; }
}

public class GraphEpisode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? SeasonCount { get; set; }
    public int EpisodeCount { get; set; }
    public int Runtime { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly Release { get; set; }
}

public class GraphPerson
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
}

public class GraphGenre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class GraphRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class GraphSubscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
}

public class GraphPrivilege
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// =============================================
// NEO4J RELATIONSHIP PROPERTY MODELS
// =============================================
// These models represent properties on relationships in Neo4j

public class ReviewRelationship
{
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public DateTime ReviewDate { get; set; }
}

public class WorkedOnRelationship
{
    public string Role { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Character { get; set; } // For actors
}

public class SubscriptionPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

// =============================================
// TRANSFORMATION HELPERS
// =============================================
public static class ApiModelExtensions
{
    // Extension methods for converting API models to graph models
    public static GraphUser ToGraphUser(this User user)
    {
        return new GraphUser
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password
        };
    }

    public static GraphProfile ToGraphProfile(this Profile profile)
    {
        return new GraphProfile
        {
            Id = profile.Id,
            Name = profile.Name,
            IsChild = profile.IsChild ?? false
        };
    }

    public static GraphWatchList ToGraphWatchList(this WatchList watchList)
    {
        return new GraphWatchList
        {
            Id = watchList.ProfileId, // WatchList uses ProfileId as primary key
            IsLocked = watchList.IsLocked ?? false
        };
    }

    public static GraphMedia ToGraphMedia(this Media media)
    {
        return new GraphMedia
        {
            Id = media.Id,
            Name = media.Name,
            Type = media.Type,
            Runtime = media.Runtime,
            Description = media.Description,
            Cover = media.Cover,
            AgeLimit = media.AgeLimit,
            Release = media.Release
        };
    }

    public static GraphEpisode ToGraphEpisode(this Episode episode)
    {
        return new GraphEpisode
        {
            Id = episode.Id,
            Name = episode.Name,
            SeasonCount = episode.SeasonCount,
            EpisodeCount = episode.EpisodeCount,
            Runtime = episode.Runtime,
            Description = episode.Description,
            Release = episode.Release
        };
    }

    public static GraphPerson ToGraphPerson(this Person person)
    {
        return new GraphPerson
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            Gender = person.Gender
        };
    }

    public static GraphGenre ToGraphGenre(this Genre genre)
    {
        return new GraphGenre
        {
            Id = genre.Id,
            Name = genre.Name,
            Description = null // API Genre doesn't have Description
        };
    }

    public static GraphRole ToGraphRole(this Role role)
    {
        return new GraphRole
        {
            Id = role.Id,
            Name = role.Name,
            Description = null // API Role doesn't have Description
        };
    }

    public static GraphSubscription ToGraphSubscription(this Subscription subscription)
    {
        return new GraphSubscription
        {
            Id = subscription.Id,
            Name = subscription.Name,
            Price = subscription.Price
        };
    }

    public static GraphPrivilege ToGraphPrivilege(this Privilege privilege)
    {
        return new GraphPrivilege
        {
            Id = privilege.Id,
            Name = privilege.Name
        };
    }
}
