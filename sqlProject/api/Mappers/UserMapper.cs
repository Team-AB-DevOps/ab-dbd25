using api.Models;
using api.Models.DTOs.Domain;
using api.Models.Sql;

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
            user.Privileges.Select(p => p.Name).ToList(),
            user.Profiles.Select(profile => profile.FromSqlEntityToDto()).ToList()
        );
    }
}
