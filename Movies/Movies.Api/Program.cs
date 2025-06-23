using Movies.Application;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseHttpsRedirection();


app.MapControllers();
app.Run();

 