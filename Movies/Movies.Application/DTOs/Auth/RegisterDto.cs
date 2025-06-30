namespace Movies.Application.DTOs.Auth;

public record RegisterDto
{
    public required string FirstName { get; init; }

    public string? LastName { get; init; } = null;

    public required string Email { get; init; }

    public required string Password { get; init; }
}