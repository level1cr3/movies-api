using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Data;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.Movies;
using Movies.Application.Services.Movies;
using Movies.Application.Email;
using Movies.Application.Settings;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddAuthentication(services);
        AddEmailConfiguration(services, configuration);
        AddServices(services, configuration);

        services.Configure<FrontendSettings>(configuration.GetSection("Frontend"));
        services.AddValidatorsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
        return services;
    }


    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConnectionString("Database") ??
                           throw new InvalidOperationException("connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dbConnection).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMovieRepository, MovieRepository>();
    }
    
    private static void AddAuthentication(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
    
    private static void AddEmailConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();
    }
    
    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMovieService,MovieService>();
    }
    

    
    
    
    
    
    
    
}