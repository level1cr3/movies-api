using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.DTOs.Movies;
using Movies.Contracts.Requests.MovieRequest;
using Movies.Contracts.Responses.MovieResponse;

namespace Movies.Api.Mappings;

public static class MovieMapping
{
    public static MovieResponse ToMovieResponse(this MovieDto movieDto)
    {
        return new MovieResponse
        {
            Id = movieDto.Id,
            Slug = movieDto.Slug,
            Title = movieDto.Title,
            YearOfRelease = movieDto.YearOfRelease
        };
    }

    public static CreateMovieCommand ToCreateMovieCommand(this CreateMovieRequest request)
    {
        return new CreateMovieCommand
        {
            Title = request.Title,
            YearOfRelease = request.YearOfRelease
        };
    }
}