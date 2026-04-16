using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Seeders;

public class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Games.Any())
        {
            var games = new List<Game>()
            {
                new Game
                {
                    Title = "Grand Theft Auto V",
                    Description = "Open world action-adventure game"
                },
                new Game
                {
                    Title = "Bloodborne",
                    Description = "Action RPG ontwikkeld door FromSoftware"
                },
                new Game
                {
                    Title = "The Last of Us",
                    Description = "Verhaalgebaseerd single-player game"
                },
                new Game
                {
                    Title = "Harry Potter en de Geheime kamer",
                    Description = "Een game van Harry Potter met 5 verschillende varianten voor verschillende platfromen"
                }
            };
            
            context.Games.AddRange(games);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var user = new User
            {
                Username = "jcmvdb",
                Email = "mvdb.jeroen@gmail.com",
                PasswordHash = "password",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        
        if (!context.Collections.Any())
        {
            var firstUser = context.Users.First();

            var collections = new List<Collection>
            {
                new Collection
                {
                    Name = "Favorieten",
                    Description = "Mijn favoriete games",
                    UserId = firstUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsPublic = false
                },
                new Collection
                {
                    Name = "Nog spelen",
                    Description = "Games die ik nog wil spelen",
                    UserId = firstUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsPublic = true
                }
            };

            context.Collections.AddRange(collections);
            await context.SaveChangesAsync();
        }

        if (!context.CollectionGames.Any())
        {
            var firstCollection = context.Collections.First();
            var secondCollection = context.Collections.Skip(1).First();
            var games = context.Games.ToList();
            
            var collectionGames = new List<CollectionGame>
            {
                new CollectionGame
                {
                    CollectionId = firstCollection.Id,
                    GameId = games[0].Id!.Value,
                    CreatedAt = DateTime.UtcNow
                },
                new CollectionGame
                {
                    CollectionId = firstCollection.Id,
                    GameId = games[1].Id!.Value,
                    CreatedAt = DateTime.UtcNow
                },
                new CollectionGame
                {
                    CollectionId = secondCollection.Id,
                    GameId = games[2].Id!.Value,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.CollectionGames.AddRange(collectionGames);
            await context.SaveChangesAsync();
        }
    }
}