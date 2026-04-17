using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Repositories;
using MijnGameKast.API.Services;
using MijnGameKast.API.Services.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data;
using MijnGameKast.API.Data.Seeders;

namespace MijnGameKast.API;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("CONNECTION_STRING is niet gevonden in het .env bestand!");
        }
        
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        
        // Add the database context
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
        builder.Services.AddScoped<ICatalogService, CatalogService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISessionRepository, SessionRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
        builder.Services.AddScoped<ICollectionService, CollectionService>();
        builder.Services.AddScoped<ICollectionGameRepository, CollectionGameRepository>();
        builder.Services.AddScoped<ICollectionGameService, CollectionGameService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        // Database seeding
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbSeeder.SeedAsync(context).Wait();
        }
        
        app.Run();
    }
}