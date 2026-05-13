using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MijnGameKast.API.Data;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Repositories;
using MijnGameKast.API.Data.Seeders;
using MijnGameKast.API.Services;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Middleware;
using Scalar.AspNetCore;

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

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "Token",
                    Description = "Voer hier je Bearer token in"
                };

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var relativePath = context.Description.RelativePath ?? string.Empty;
                var method = context.Description.HttpMethod?.ToUpperInvariant();

                var requiresAuth =
                    relativePath.StartsWith("api/collection", StringComparison.OrdinalIgnoreCase) ||
                    (relativePath.Equals("api/auth/logout", StringComparison.OrdinalIgnoreCase) && method == "POST") ||
                    (relativePath.Equals("api/auth/me", StringComparison.OrdinalIgnoreCase) && method == "GET");

                if (requiresAuth)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                    });
                }

                return Task.CompletedTask;
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

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
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddScoped<IPlatformService, PlatformService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontenDev", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        
        app.UseHttpsRedirection();
        app.UseCors("FrontenDev");
        app.UseAuthorization();

        app.UseMiddleware<TokenMiddleware>();
        
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbSeeder.SeedAsync(dbContext).Wait();
        }

        app.Run();
    }
}