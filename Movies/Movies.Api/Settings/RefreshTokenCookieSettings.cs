namespace Movies.Api.Settings;

public sealed class RefreshTokenCookieSettings
{
    public required bool Secure { get; init; }
    
    public required string SameSite { get; init; }
    
    public required string Path { get; init; }
    
}