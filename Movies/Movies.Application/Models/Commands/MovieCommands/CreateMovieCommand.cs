namespace Movies.Application.Models.Commands.MovieCommands;

public record CreateMovieCommand
{
    public string Title { get; init; } = string.Empty;

    public int YearOfRelease { get; init; }
}