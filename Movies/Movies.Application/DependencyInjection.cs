using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Data;
using Movies.Application.Repositories;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        
        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Database") ??
                           throw new InvalidOperationException("connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnection).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}