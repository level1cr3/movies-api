namespace Movies.Application.Data.Seeder;

public record AdminInfo
{
    public required string Email { get; init; }
    
    public required string Password { get; init; }
}