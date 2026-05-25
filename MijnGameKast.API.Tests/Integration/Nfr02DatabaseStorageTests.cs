using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Enums;
using MijnGameKast.API.Data.Repositories;

namespace MijnGameKast.API.Tests.Integration;

public class Nfr02DatabaseStorageTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    
    // Non-functional requirement: NFR02 - Data opgeslagen in een database
    // Testcase: UTC-NFR02-01
    // Doel: Controleren of een nieuw aangemaakte collectie wordt opgeslagen en opnieuw opgehaald kan worden.
    [Fact]
    public async Task UTC_NFR02_01_AddAsync_ShouldSaveCollection_AndGetByIdAsyncShouldReturnSameCollection()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new CollectionRepository(dbContext);

        var collection = new Collection
        {
            Name = "Mijn RPG collectie",
            Description = "Collectie met mijn favoriete RPG games",
            UserId = 1,
            IsPublic = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var savedCollection = await repository.AddAsync(collection);
        var retrievedCollection = await repository.GetByIdAsync(savedCollection.Id);

        // Assert
        retrievedCollection.Should().NotBeNull();
        retrievedCollection!.Id.Should().Be(savedCollection.Id);
        retrievedCollection.Name.Should().Be(collection.Name);
        retrievedCollection.Description.Should().Be(collection.Description);
        retrievedCollection.UserId.Should().Be(collection.UserId);
    }
    
    // Non-functional requirement: NFR02 - Data opgeslagen in een database
    // Testcase: UTC-NFR02-02
    // Doel: Controleren of een nieuw toegevoegde catalogusgame wordt opgeslagen en opnieuw opgehaald kan worden.
    [Fact]
    public async Task UTC_NFR02_02_AddGameAsync_ShouldSaveGame_AndGetByIdAsyncShouldReturnSameGame()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new CatalogRepository(dbContext);

        var game = new Game
        {
            Title = "Elden Ring",
            Description = "Open world action RPG",
            UserId = 1,
            Status = GameStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var savedGame = await repository.AddGameAsync(game);
        var retrievedGame = await repository.GetByIdAsync(savedGame.Id!.Value);

        // Assert
        retrievedGame.Should().NotBeNull();
        retrievedGame!.Id.Should().Be(savedGame.Id);
        retrievedGame.Title.Should().Be(game.Title);
        retrievedGame.Description.Should().Be(game.Description);
        retrievedGame.UserId.Should().Be(game.UserId);
        retrievedGame.Status.Should().Be(game.Status);
    }

    // Non-functional requirement: NFR02 - Data opgeslagen in een database
    // Testcase: UTC-NFR02-03
    // Doel: Controleren of gewijzigde gegevens correct worden opgeslagen in de database.
    [Fact]
    public async Task UTC_NFR02_03_UpdateGameAsync_ShouldSaveChangedGameData()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new CatalogRepository(dbContext);

        var game = new Game
        {
            Title = "Oude titel",
            Description = "Oude beschrijving",
            UserId = 1,
            Status = GameStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var savedGame = await repository.AddGameAsync(game);

        savedGame.Title = "Nieuwe titel";
        savedGame.Description = "Nieuwe beschrijving";

        // Act
        var updateResult = await repository.UpdateGameAsync(savedGame);
        var retrievedGame = await repository.GetByIdAsync(savedGame.Id!.Value);

        // Assert
        updateResult.Should().BeTrue();

        retrievedGame.Should().NotBeNull();
        retrievedGame!.Title.Should().Be("Nieuwe titel");
        retrievedGame.Description.Should().Be("Nieuwe beschrijving");
    }

    // Non-functional requirement: NFR02 - Data opgeslagen in een database
    // Testcase: UTC-NFR02-04
    // Doel: Controleren of verwijderde gegevens niet opnieuw worden opgehaald.
    [Fact]
    public async Task UTC_NFR02_04_DeleteAsync_ShouldRemoveCollection_AndGetByIdAsyncShouldReturnNull()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var repository = new CollectionRepository(dbContext);

        var collection = new Collection
        {
            Name = "Te verwijderen collectie",
            Description = "Deze collectie wordt verwijderd",
            UserId = 1,
            IsPublic = false,
            CreatedAt = DateTime.UtcNow
        };

        var savedCollection = await repository.AddAsync(collection);

        // Act
        var deleteResult = await repository.DeleteAsync(savedCollection.Id);
        var retrievedCollection = await repository.GetByIdAsync(savedCollection.Id);

        // Assert
        deleteResult.Should().BeTrue();
        retrievedCollection.Should().BeNull();
    }
}