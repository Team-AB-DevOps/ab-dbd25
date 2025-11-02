using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class UserMapper
{
    public static UserDto FromSqlEntityToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Subscriptions.Select(s => s.Id).ToList(),
            user.Privileges.Select(p => p.Name).ToList()
            );
    }
}