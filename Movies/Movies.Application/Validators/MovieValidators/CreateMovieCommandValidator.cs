using FluentValidation;
using Movies.Application.Models.Commands.MovieCommands;

namespace Movies.Application.Validators.MovieValidators;

public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(200);

        RuleFor(c => c.YearOfRelease).InclusiveBetween(1900, DateTime.UtcNow.Year);
    }
}