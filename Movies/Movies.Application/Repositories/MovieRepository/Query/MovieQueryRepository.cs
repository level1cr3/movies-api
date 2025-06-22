using Microsoft.EntityFrameworkCore;
using Movies.Application.Data;
using Movies.Application.Mappings;
using Movies.Application.Models.Aggregates.MovieAggregates;

namespace Movies.Application.Repositories.MovieRepository.Query;

internal sealed class MovieQueryRepository(ApplicationDbContext db) : IMovieQueryRepository
{
    public async Task<MovieAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Movie.Where(m => m.Id == id)
            .Select(m => m.ToMovieAggregate())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MovieAggregate?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await db.Movie.Where(m => m.Slug == slug)
            .Select(m => m.ToMovieAggregate())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MovieAggregateList> GetAllAsync(CancellationToken cancellationToken = default)
    {
       var movies = await db.Movie.ToListAsync(cancellationToken);
       return movies.ToMovieAggregateList();
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