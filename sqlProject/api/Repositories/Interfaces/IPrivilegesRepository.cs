using api.Models;

namespace api.Interfaces;

public interface IPrivilegesRepository
{
    Task<Privilege?> GetByName(string name);
}
