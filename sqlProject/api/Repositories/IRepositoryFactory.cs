namespace api.Repositories;

public interface IRepositoryFactory
{
    IRepository GetRepository(string tenant);
}
