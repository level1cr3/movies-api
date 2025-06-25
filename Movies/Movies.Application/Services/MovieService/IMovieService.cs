using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.DTOs.Movies;

namespace Movies.Application.Services.MovieService;

public interface IMovieService
{
    Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default);
}