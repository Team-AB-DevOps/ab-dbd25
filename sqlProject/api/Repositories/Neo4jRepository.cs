using api.DTOs;
using Neo4j.Driver;

namespace api.Repositories;

public class Neo4jRepository(IDriver driver) : IRepository
{
    public async Task<List<MediaDTO>> GetAllMedias()
    {
        var result = await driver.ExecutableQuery(@"
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