using Movies.Application.Models.Aggregates.MovieAggregates;
using Movies.Application.Models.Commands.MovieCommands;
using Movies.Contracts.Requests.MovieRequest;
using Movies.Contracts.Responses.MovieResponse;

namespace Movies.Api.Mappings;

public static class MovieMapping
{
    public static MovieResponse ToMovieResponse(this MovieAggregate movieAggregate)
    {
        return new MovieResponse
        {
            Id = movieAggregate.Id,
            Slug = movieAggregate.Slug,
            Title = movieAggregate.Title,
            YearOfRelease = movieAggregate.YearOfRelease
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