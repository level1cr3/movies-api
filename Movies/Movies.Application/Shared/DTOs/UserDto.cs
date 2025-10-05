namespace Movies.Application.Shared.DTOs;

public sealed record UserDto(Guid Id, string Name, string Email, IList<string> Roles);