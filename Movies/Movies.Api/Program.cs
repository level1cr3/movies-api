using Movies.Api.Middleware;
using Movies.Api.OpenApi.Transformers;
using Movies.Application;
using Movies.Application.Data.Seeder;
using Scalar.AspNetCore;
using Serilog;

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

// builder.Services.Configure<>()

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


app.MapControllers();
app.Run();

 