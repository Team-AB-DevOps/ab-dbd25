using api.Data;
using api.Interfaces;
using api.Models;
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
