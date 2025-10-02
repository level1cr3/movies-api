namespace Movies.Application.Shared.DTOs;

public sealed record UserDto(string Name, string Email, IList<string> Roles);