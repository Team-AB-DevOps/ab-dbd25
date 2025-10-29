using api.ExceptionHandlers;

namespace api.Repositories;

public class RepositoryFactory(IServiceProvider provider) : IRepositoryFactory
{
    public IRepository GetRepository(string tenant)
    {
        return tenant.ToLower() switch
        {
            "sql" => provider.GetRequiredService<SqlRepository>(),
            "mongo" => provider.GetRequiredService<MongoRepository>(),
            // "neo4j" => provider.GetRequiredService<Neo4jRepository>(),
            _ => provider.GetRequiredService<SqlRepository>()
        };
    }
}