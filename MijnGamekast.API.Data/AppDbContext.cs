using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CollectionGame> CollectionGames => Set<CollectionGame>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<CollectionGame>()
            .HasKey(cg => new { cg.CollectionId, cg.GameId });
    }
}