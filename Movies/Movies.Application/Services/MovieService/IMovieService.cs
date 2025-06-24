using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.DTOs.Movies;
using Movies.Application.Models.Queries.MovieQueries;

namespace Movies.Application.Services.MovieService;

public interface IMovieService
{
    Task<MovieDto?> GetByIdAsync(GetMovieByIdQuery query, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateMovieCommand command, CancellationToken cancellationToken = default);
}