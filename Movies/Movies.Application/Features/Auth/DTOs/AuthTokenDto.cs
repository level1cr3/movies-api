namespace Movies.Application.Features.Auth.DTOs;

public record AuthTokenDto
{
    public required string AccessToken { get; init; }
    
    public required int ExpiresIn { get; init; }
    
    public required string TokenType { get; init; }
    
    public required string RefreshToken { get; init; }
}