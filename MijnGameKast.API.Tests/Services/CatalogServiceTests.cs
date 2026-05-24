using FluentAssertions;
using Moq;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Services;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Tests.Services;

public class CatalogServiceTests
{
    private readonly Mock<ICatalogRepository> _catalogRepositoryMock = new();
    private readonly Mock<IGamePlatformRepository> _gamePlatformRepositoryMock = new();
    private readonly Mock<IGameGenreRepository> _gameGenreRepositoryMock = new();
    private readonly Mock<ICollectionGameRepository> _collectionGameRepositoryMock = new();

    private CatalogService CreateService()
    {
        return new CatalogService(
            _catalogRepositoryMock.Object,
            _gamePlatformRepositoryMock.Object,
            _gameGenreRepositoryMock.Object,
            _collectionGameRepositoryMock.Object
        );
    }
    
    // Use case: UC08 - Catalogusgame verwijderen
    // Testcase: UTC-UC08-01
    // Doel: Controleren of een moderator een bestaande catalogusgame kan verwijderen.
    [Fact]
    public async Task UTC_UC08_01_DeleteGameAsync_ShouldDeleteGame_WhenGameExists()
    {
        // Arrange
        const int gameId = 1;

        var game = new Game
        {
            Id = gameId,
            Title = "Elden Ring",
            Description = "Open world RPG"
        };
        
        _catalogRepositoryMock
            .Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync(game);

        _gamePlatformRepositoryMock
            .Setup(repo => repo.DeleteByGameIdAsync(gameId))
            .ReturnsAsync(1);
        
        _gameGenreRepositoryMock
            .Setup(repo => repo.DeleteByGameIdAsync(gameId))
            .ReturnsAsync(1);
        
        _collectionGameRepositoryMock
            .Setup(repo => repo.DeleteByGameIdAsync(gameId))
            .ReturnsAsync(1);
        
        _catalogRepositoryMock
            .Setup(repo => repo.DeleteGameAsync(gameId))
            .ReturnsAsync(true);
        
        var service = CreateService();
        
        // Act
        var result = await service.DeleteGameAsync(gameId);

        // Assert
        result.Success.Should().BeTrue();
        result.Type.Should().Be(ServiceResultType.Success);
        
        _gamePlatformRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(gameId),
            Times.Once
        );

        _gameGenreRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(gameId),
            Times.Once
        );

        _collectionGameRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(gameId),
            Times.Once
        );

        _catalogRepositoryMock.Verify(
            repo => repo.DeleteGameAsync(gameId),
            Times.Once
        );
    }
    
    // Use case: UC08 - Catalogusgame verwijderen
    // Testcase: UTC-UC08-03
    // Doel: Controleren wat er gebeurt wanneer een moderator een catalogusgame probeert te verwijderen die niet bestaat.
    [Fact]
    public async Task UTC_UC08_03_DeleteGameAsync_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        const int gameId = 1;
        
        _catalogRepositoryMock
            .Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);
        
        var service = CreateService();

        // Act
        var result = await service.DeleteGameAsync(gameId);

        // Assert
        result.Success.Should().BeFalse();
        result.Type.Should().Be(ServiceResultType.NotFound);

        _gamePlatformRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(It.IsAny<int>()),
            Times.Never
        );

        _gameGenreRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(It.IsAny<int>()),
            Times.Never
        );

        _collectionGameRepositoryMock.Verify(
            repo => repo.DeleteByGameIdAsync(It.IsAny<int>()),
            Times.Never
        );

        _catalogRepositoryMock.Verify(
            repo => repo.DeleteGameAsync(It.IsAny<int>()),
            Times.Never
        );
    }
    
    // Use case: UC16 - Catalogusgame wijzigen
    // Testcase: UTC-UC16-01
    // Doel: Controleren of een moderator een bestaande catalogusgame kan wijzigen.
    [Fact]
    public async Task UTC_UC16_01_UpdateGameAsync_ShouldUpdateGame_WhenGameExists()
    {
        // Arrange
        const int gameId = 1;

        var existingGame = new Game
        {
            Id = gameId,
            Title = "Elden ring",
            Description = "Open world RPG"
        };

        var request = new UpdateGameRequest()
        {
            Title = "Nieuwe titel",
            Description = "Nieuwe Beschrijving"
        };
        
        _catalogRepositoryMock
            .Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync(existingGame);
        
        _catalogRepositoryMock
            .Setup(repo => repo.UpdateGameAsync(It.IsAny<Game>()))
            .ReturnsAsync(true);

        var service = CreateService();
        
        // Act
        var result = await service.UpdateGameAsync(gameId, request);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Type.Should().Be(ServiceResultType.Success);
        
        existingGame.Title.Should().Be(request.Title);
        existingGame.Description.Should().Be(request.Description);

        _catalogRepositoryMock.Verify(
            repo => repo.GetByIdAsync(gameId),
            Times.Once
        );
        
        _catalogRepositoryMock.Verify(
            repo => repo.UpdateGameAsync(It.Is<Game>(game =>
                game.Id == gameId &&
                game.Title == request.Title &&
                game.Description == request.Description
                )), Times.Once);
    }
    
    // Use case: UC16 - Catalogusgame wijzigen
    // Testcase: UTC-UC16-04
    // Doel: Controleren of wijzigen van een niet-bestaande catalogusgame een foutresultaat geeft.
    [Fact]
    public async Task UTC_UC16_04_UpdateGameAsync_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        const int gameId = 999;

        var request = new UpdateGameRequest
        {
            Title = "Nieuwe titel",
            Description = "Nieuwe beschrijving"
        };

        _catalogRepositoryMock
            .Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        var service = CreateService();

        // Act
        var result = await service.UpdateGameAsync(gameId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Type.Should().Be(ServiceResultType.NotFound);

        _catalogRepositoryMock.Verify(
            repo => repo.GetByIdAsync(gameId),
            Times.Once
        );

        _catalogRepositoryMock.Verify(
            repo => repo.UpdateGameAsync(It.IsAny<Game>()),
            Times.Never
        );
    }
}