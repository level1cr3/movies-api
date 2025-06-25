using Movies.Application.DTOs.Movies;
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

    public static CreateMovieDto ToCreateMovieDto(this CreateMovieRequest request)
    {
        return new CreateMovieDto
        {
            Title = request.Title,
            YearOfRelease = request.YearOfRelease
        };
    }
}