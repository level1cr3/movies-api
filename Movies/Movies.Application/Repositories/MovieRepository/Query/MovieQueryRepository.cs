using Microsoft.EntityFrameworkCore;
using Movies.Application.Data;
using Movies.Application.Mappings;
using Movies.Application.Models.DTOs.Movies;

namespace Movies.Application.Repositories.MovieRepository.Query;

internal sealed class MovieQueryRepository(ApplicationDbContext db) : IMovieQueryRepository
{
    public async Task<MovieDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movies.Where(m => m.Id == id)
            .Select(m => m.ToMovieDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MovieDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movies.Where(m => m.Slug == slug)
            .Select(m => m.ToMovieDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
       var movies = await db.Movies.ToListAsync(cancellationToken);
       return movies.ToMovieDtoList();
    }
    
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movies.AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movies.AnyAsync(m => m.Slug == slug, cancellationToken);
    }
}