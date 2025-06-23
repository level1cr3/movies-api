using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Data;
using Movies.Application.Repositories;
using Movies.Application.Repositories.MovieRepository.Command;
using Movies.Application.Repositories.MovieRepository.Query;
using Movies.Application.Services.MovieService;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddServices(services);
        
        return services;
    }
    
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Database") ??
                           throw new InvalidOperationException("connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnection).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMovieQueryRepository, MovieQueryRepository>();
        services.AddScoped<IMovieCommandRepository, MovieCommandRepository>();
    }
    
    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IMovieService,MovieService>();
    }

    
}