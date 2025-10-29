using api.DTOs;
using Neo4j.Driver;

namespace api.Repositories;

public class Neo4jRepository : IRepository
{
    private readonly IDriver _driver;

    public Neo4jRepository(IConfiguration configuration)
    {
        var neo4JConfig = configuration.GetSection("Neo4j");
        var uri = neo4JConfig["Uri"] ?? throw new ArgumentNullException("Neo4j:Uri is required");
        var user = neo4JConfig["User"] ?? throw new ArgumentNullException("Neo4j:User is required");
        var password =
            neo4JConfig["Password"]
            ?? throw new ArgumentNullException("Neo4j:Password is required");

        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
    }

    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var result = await _driver.ExecutableQuery(@"
        MATCH (media:Media) RETURN media
    ")
            .WithConfig(new QueryConfig(database: "neo4j"))
            .ExecuteAsync();

        // Map nodes to MediaDTO
        return result.Result.Select(record => record["media"].As<INode>())
            .Select(node =>
                new MediaDTO(
                    node.Properties["id"].As<int>(),
                    node.Properties["name"].As<string>(),
                    node.Properties["type"].As<string>(),
                    node.Properties["runtime"].As<int>(),
                    node.Properties["description"].As<string>(),
                    node.Properties["cover"].As<string>(),
                    node.Properties["ageLimit"].As<int>(),
                    DateOnly.Parse(node.Properties["release"].As<string>())
                )
            )
            .ToList();
    }
}