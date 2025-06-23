using Movies.Api.Middleware;
using Movies.Application;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();




var app = builder.Build();

app.UseExceptionHandler();
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

 