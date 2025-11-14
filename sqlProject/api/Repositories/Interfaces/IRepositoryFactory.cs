namespace api.Repositories.Interfaces;

public interface IRepositoryFactory
{
    IRepository GetRepository(string tenant);
}
