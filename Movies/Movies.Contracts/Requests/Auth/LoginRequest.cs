namespace Movies.Contracts.Requests.Auth;

public record LoginRequest
{
    public required string Username { get; init; } 
    public required string Password { get; init; } 
}