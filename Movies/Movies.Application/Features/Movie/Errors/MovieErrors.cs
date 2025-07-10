using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Movie.Errors;

public static class MovieErrors
{
    public static readonly AppError NotFound = new("Movie.NotFound","Movie not found.");
    
    public static readonly AppError SaveFailed = new("Movie.SaveFailed","An error occurred while saving the movie.");
}