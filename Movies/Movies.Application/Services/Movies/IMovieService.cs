using Movies.Application.DTOs.Movies;

namespace Movies.Application.Services.Movies;

public interface IMovieService
{
    Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default);
}