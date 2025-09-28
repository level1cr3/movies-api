namespace Movies.Application.Features.Auth.DTOs;

public sealed record UserDto(string Name, string Email, List<string> Roles);