namespace Movies.Application.Models.Commands.MovieCommands;

public record UpdateMovieDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int YearOfRelease { get; init; }
}

// reason for not making these properties required is that i will do all the validation in services 
// via fluent validation.