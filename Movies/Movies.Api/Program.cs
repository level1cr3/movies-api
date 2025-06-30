using Movies.Api.Middleware;
using Movies.Application;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

// builder.Services.Configure<>()

var app = builder.Build();

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

 