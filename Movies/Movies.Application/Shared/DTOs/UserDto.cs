namespace Movies.Application.Shared.DTOs;

public sealed record UserDto(string Name, string Email, List<string> Roles);