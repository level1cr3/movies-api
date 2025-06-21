using Microsoft.EntityFrameworkCore;
using Movies.Application.Data;
using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Query;

public class MovieQueryRepository(ApplicationDbContext db) : IMovieQueryRepository
{
    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movie.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movie.FirstOrDefaultAsync(m => m.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Movie.ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movie.AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movie.AnyAsync(m => m.Slug == slug, cancellationToken);
    }
}