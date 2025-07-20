namespace Movies.Contracts.Requests.Auth;

public record RefreshTokenRequest
{
    public required string Token { get; init; }
}