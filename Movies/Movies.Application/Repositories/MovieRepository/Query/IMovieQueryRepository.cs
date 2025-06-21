using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Query;

public interface IMovieQueryRepository
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}