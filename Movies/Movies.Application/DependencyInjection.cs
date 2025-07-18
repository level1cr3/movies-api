using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Data;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Repositories;
using Movies.Application.Data.Repositories.JwtRefreshToken;
using Movies.Application.Data.Repositories.Movies;
using Movies.Application.Data.Seeder;
using Movies.Application.Email;
using Movies.Application.Features.Auth.Services;
using Movies.Application.Features.Movie.Services;
using Movies.Application.Settings;
using Movies.Application.Shared.Foundation;

namespace Movies.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddEmailConfiguration(services, configuration);
        AddAuthentication(services, configuration);
        AddServices(services);

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

        services.Configure<AdminInfo>(configuration.GetSection("DefaultAdmin"));
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    private static void AddEmailConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
                          ?? throw new InvalidOperationException("Jwt configuration is not available.");

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IRequestContextService, RequestContextService>();
    }
}