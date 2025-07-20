namespace Movies.Contracts.Responses.Auth;

public sealed record TokenResponse
{
    public required string AccessToken { get; init; }
    
    public required int ExpiresIn { get; init; }
    
    public required string TokenType { get; init; }
    
    public required string RefreshToken { get; init; }
}