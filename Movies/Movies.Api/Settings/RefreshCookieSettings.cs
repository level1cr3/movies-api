namespace Movies.Api.Settings;

public sealed class RefreshCookieSettings
{
    public required bool Secure { get; init; }
    
    public required string SameSite { get; init; }
    
    public required string Domain { get; init; }
}