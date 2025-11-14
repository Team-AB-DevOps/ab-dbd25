using api.Models.Sql;

namespace api.Repositories.Interfaces;

public interface IPrivilegesRepository
{
    Task<Privilege?> GetByName(string name);
}
