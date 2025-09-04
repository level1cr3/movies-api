namespace Movies.Application.Features.Auth.DTOs;

public record AuthTokenDto
{
    public required string AccessToken { get; init; }
    
    public required int ExpiresIn { get; init; }
    
    public required string TokenType { get; init; }
    
    public required string RefreshToken { get; init; }
    
    public required DateTime RefreshTokenExpiresAt { get; init; }
    
    internal Guid RefreshTokenId { get; init; } 
    //Note: only need it internally. so during token revoke i could pass who revoke it
    // doesn't need to be exposed to the client
}