namespace Movies.Contracts.Requests.Auth;

public record RegisterRequest
{
    public required string FirstName { get; init; }

    public string? LastName { get; init; } = null;

    public required string Email { get; init; }

    public required string Password { get; init; }
}

// confirm password is purely client side responsibility.