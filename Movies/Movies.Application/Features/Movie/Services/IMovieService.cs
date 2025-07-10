using Movies.Application.Features.Movie.DTOs;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Movie.Services;

public interface IMovieService
{
    Task<Result<MovieDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<Guid>> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default);
}