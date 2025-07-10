using FluentValidation;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.Movies;
using Movies.Application.Features.Auth.Mappings;
using Movies.Application.Features.Movie.DTOs;
using Movies.Application.Features.Movie.Errors;
using Movies.Application.Features.Movie.Mappings;
using Movies.Application.Shared.Foundation;

namespace Movies.Application.Features.Movie.Services;

internal sealed class MovieService(
    IMovieRepository movieRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateMovieDto> createMovieDtoValidator) : IMovieService
{
    public async Task<Result<MovieDto>> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var movieDto = await movieRepository.GetByIdAsync(id, cancellationToken);

        return movieDto is null 
            ? Result.Failure<MovieDto>([MovieErrors.NotFound]) 
            : Result.Success(movieDto);
    }

    public async Task<Result<Guid>> CreateAsync(CreateMovieDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await createMovieDtoValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<Guid>(validationResult.Errors.ToAppErrors());
        }
        
        var movie = dto.ToMovie();
        movieRepository.Create(movie);
        
        var rowsAffected = await unitOfWork.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0 ? Result.Success(movie.Id) : Result.Failure<Guid>([MovieErrors.SaveFailed]);
        
        
    }
    
    
    
}