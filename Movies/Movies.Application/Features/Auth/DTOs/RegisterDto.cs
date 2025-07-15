namespace Movies.Application.Features.Auth.DTOs;

public record RegisterDto
{
    public required string FirstName { get; init; }

    public string? LastName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}