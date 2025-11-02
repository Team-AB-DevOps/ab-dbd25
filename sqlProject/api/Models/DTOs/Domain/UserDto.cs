using api.Models;

namespace api.DTOs;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    List<int> Subscriptions,
    List<string> Privileges
    );
