using Movies.Application.Data;
using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Command;

internal sealed class MovieCommandRepository(ApplicationDbContext db) : IMovieCommandRepository
{
    public void Create(Movie movie)
    {
        db.Movie.Add(movie);
    }

    public void Update(Movie movie)
    {
        db.Movie.Update(movie);
    }

    public void Delete(Movie movie)
    {
        db.Movie.Remove(movie);
    }
}