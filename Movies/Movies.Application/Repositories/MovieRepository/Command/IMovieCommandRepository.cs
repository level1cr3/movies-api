using Movies.Application.Models.Entities;

namespace Movies.Application.Repositories.MovieRepository.Command;

internal interface IMovieCommandRepository
{
    void Create(Movie movie, CancellationToken cancellationToken = default);

    void Update(Movie movie, CancellationToken cancellationToken = default);

    void Delete(Movie movie, CancellationToken cancellationToken = default);
}