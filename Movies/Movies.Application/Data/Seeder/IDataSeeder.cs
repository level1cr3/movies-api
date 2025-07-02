using Microsoft.Extensions.Options;

namespace Movies.Application.Data.Seeder;

public interface IDataSeeder
{
    Task SeedAsync();
}