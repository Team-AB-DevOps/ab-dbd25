using api.Data;
using api.Models;
using api.Models.Sql;
using api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class PrivilegesRepository : IPrivilegesRepository
{
    private readonly DataContext _dataContext;

    public PrivilegesRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<Privilege> GetByName(string name)
    {
        return _dataContext.Privileges.FirstOrDefaultAsync(p => p.Name == name);
    }
}
