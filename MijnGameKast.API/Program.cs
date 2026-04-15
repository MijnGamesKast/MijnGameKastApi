using Microsoft.AspNetCore.Mvc;
using MijnGamekast.API.Data.Interfaces;
using MijnGamekast.API.Data.Repositories;
using MijnGameKast.API.Services;
using MijnGameKast.API.Services.interfaces;

namespace MijnGameKast.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
        builder.Services.AddScoped<ICatalogService, CatalogService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}