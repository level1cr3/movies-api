using Movies.Application.Features.Movie.DTOs;

namespace Movies.Application.Features.Movie.Services;

public interface IMovieService
{
    Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default);
}