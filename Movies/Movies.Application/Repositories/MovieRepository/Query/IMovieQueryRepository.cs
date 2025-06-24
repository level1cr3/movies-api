using Movies.Application.Models.DTOs.Movies;

namespace Movies.Application.Repositories.MovieRepository.Query;

internal interface IMovieQueryRepository
{
    Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<MovieDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<MovieDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}