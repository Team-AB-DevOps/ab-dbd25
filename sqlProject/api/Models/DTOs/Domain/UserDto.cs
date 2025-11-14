namespace api.Models.DTOs.Domain;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    List<int> Subscriptions,
    List<string> Privileges,
    List<ProfileDto> Profiles
);
