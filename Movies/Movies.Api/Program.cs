using Microsoft.Net.Http.Headers;
using Movies.Api.Middleware;
using Movies.Api.OpenApi.Transformers;
using Movies.Application;
using Movies.Application.Data.Seeder;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.IdentityModel.Protocols.Configuration;
using Movies.Api.Services;
using Movies.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<DefaultErrorResponsesTransformer>();
});

builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var allowedOrigins=builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                   ?? throw new InvalidConfigurationException("Allowed origins is not configured");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy => policy.WithOrigins(allowedOrigins)
        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, HeaderNames.Accept)
        .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete)
        .AllowCredentials() // this is done for refresh token. It will be stored in browser cookie with httpOnly true. 
    );
});

builder.Services.Configure<RefreshTokenCookieSettings>(builder.Configuration.GetSection("RefreshTokenCookieSettings"));
builder.Services.AddScoped<IRefreshTokenCookieService, RefreshTokenCookieService>();

var app = builder.Build();

using var scope = app.Services.CreateScope(); 
var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
await seeder.SeedAsync();

// var seeder = app.Services.GetRequiredService<IDataSeeder>();
// await seeder.SeedAsync(); // this is bad and can cause issue if DataSeeder depends on scoped services. use it only for singleton

/*
 CreateScope() ensures that any scoped dependencies used inside your IDataSeeder 
 (e.g. UserManager, RoleManager, DbContext, etc.) are resolved properly within a scoped lifetime.

 Without CreateScope(), calling app.Services.GetRequiredService<>() resolves services from the root container, 
 which is not safe for scoped services.
 
 */

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    // default path is /openapi/v1.json for mapOpenApi();
    app.MapOpenApi(pattern:"api/document.json"); 
    app.MapScalarApiReference(options =>
    {
        options.Title = "Movies API";
        options.OpenApiRoutePattern = "/api/document.json";
        
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

 