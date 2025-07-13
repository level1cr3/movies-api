namespace Movies.Application.Settings;

public record JwtSettings
{
    public required string Secret { get; init; }
    
    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
    
    public required int ExpiryMinutes { get; init; }    
    
    public required int RefreshTokenExpiryDays { get; init; }
}