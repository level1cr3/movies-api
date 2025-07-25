namespace Movies.Application.Settings;

internal class FrontendSettings
{
    public required string BaseUrl { get; init; }
    
    public required string EmailConfirmationPath { get; init; }
    
    public required string ResetPasswordPath { get; init; }

}