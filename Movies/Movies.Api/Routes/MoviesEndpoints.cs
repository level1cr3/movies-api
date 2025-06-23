namespace Movies.Api.Routes;

public static class MoviesEndpoints
{
    private const string Base = $"{ApiEndpointConstants.ApiBase}/Movies";

    public const string Create = Base;
    public const string Get = $"{Base}/{{id:guid}}";
    
}