using System.Text.RegularExpressions;
using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.DTOs.Movies;
using Movies.Application.Models.Entities;

namespace Movies.Application.Mappings;

internal static class MovieMapping
{
    internal static MovieDto ToMovieDto(this Movie movie)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Slug = movie.Slug
        };
    }

    internal static IEnumerable<MovieDto> ToMovieDtoList(this IEnumerable<Movie> movies)
    {
       return movies.Select(m => m.ToMovieDto());
    }

    internal static Movie ToMovie(this CreateMovieCommand movieCommand)
    {
        return new Movie
        {
            Id = Guid.CreateVersion7(),
            Slug = GetSlug(movieCommand.Title,movieCommand.YearOfRelease),
            Title = movieCommand.Title,
            YearOfRelease = movieCommand.YearOfRelease
        };
    }

    private static string GetSlug(string title, int year)
    {
        // If it is not the character in regex replace them with empty string.
        var input = Regex.Replace(title, @"[^a-zA-Z0-9\s]", string.Empty).Trim().ToLower();
        
        // replace empty strings with hyphen (-)
        input = Regex.Replace(input, @"\s+", "-");

        return $"{input}-{year}";
    }



}