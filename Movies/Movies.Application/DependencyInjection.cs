using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Data;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Database") ??
                           throw new InvalidOperationException("connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dbConnection).UseSnakeCaseNamingConvention());
        
        return services;
    }
}