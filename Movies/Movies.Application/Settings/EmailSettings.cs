namespace Movies.Application.Settings;

internal record EmailSettings
{
    
    public required string From { get; init; }

    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string Username { get; init; }
    
    public required string Password { get; init; }
    
}