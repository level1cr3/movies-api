using FluentValidation;
using Movies.Application.Features.Movie.DTOs;

namespace Movies.Application.Features.Movie.Validators;

public class CreateMovieDtoValidator : AbstractValidator<CreateMovieDto>
{
    public CreateMovieDtoValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);

        RuleFor(c => c.YearOfRelease).InclusiveBetween(1900, DateTime.UtcNow.Year);
    }
}