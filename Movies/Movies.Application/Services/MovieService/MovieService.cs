using FluentValidation;
using Movies.Application.Mappings;
using Movies.Application.Models.Aggregates.MovieAggregates;
using Movies.Application.Models.Commands.MovieCommands;
using Movies.Application.Models.Queries.MovieQueries;
using Movies.Application.Repositories;
using Movies.Application.Repositories.MovieRepository.Command;
using Movies.Application.Repositories.MovieRepository.Query;

namespace Movies.Application.Services.MovieService;

internal sealed class MovieService(
    IMovieQueryRepository queryRepository,
    IMovieCommandRepository commandRepository,
    IUnitOfWork unitOfWork,
    IValidator<GetMovieByIdQuery> getMovieByIdValidator,
    IValidator<CreateMovieCommand> createMovieValidator) : IMovieService
{
    public async Task<MovieAggregate?> GetByIdAsync(GetMovieByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        await getMovieByIdValidator.ValidateAndThrowAsync(query, cancellationToken: cancellationToken);

        return await queryRepository.GetByIdAsync(query.Id, cancellationToken);
    }

    public async Task<Guid?> CreateAsync(CreateMovieCommand command, CancellationToken cancellationToken = default)
    {
        await createMovieValidator.ValidateAndThrowAsync(command, cancellationToken);
        
        var movie = command.ToMovie();
        commandRepository.Create(movie);
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);
        return result > 0 ? movie.Id : null;
    }
    
}