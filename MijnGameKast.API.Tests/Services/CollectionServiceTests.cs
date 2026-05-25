using FluentAssertions;
using Moq;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services;

namespace MijnGameKast.API.Tests.Services;

public class CollectionServiceTests
{
    private readonly Mock<ICollectionRepository> _collectionRepositoryMock = new();

    private CollectionService CreateService()
    {
        return new CollectionService(
            _collectionRepositoryMock.Object
        );
    }
    
    // Use case: UC03 - Persoonlijke collectie bekijken
    // Testcase: UTC-UC03-01
    // Doel: Controleren of alleen collecties van de ingelogde gebruiker worden teruggegeven.
    [Fact]
    public async Task UTC_UC03_01_GetMyCollectionsAsync_ShouldReturnOnlyCollectionsFromLoggedInUser_WhenUserIdIsGiven()
    {
        // Arrange
        int userId = 1; // Gamer A

        var allCollections = new List<Collection>
        {
            new Collection
            {
                Id = 1,
                Name = "Collectie van Gamer A",
                Description = "Eigen collectie",
                UserId = 1,
                IsPublic = false
            },
            new Collection
            {
                Id = 2,
                Name = "Collectie van Gamer B",
                Description = "Collectie van andere gebruiker",
                UserId = 2,
                IsPublic = false
            },
            new Collection
            { 
                Id = 3, 
                Name = "Openbare collectie van Gamer A", 
                Description = "Openbare collectie van Gamer A", 
                UserId = 1, 
                IsPublic = true
            }
        };

        _collectionRepositoryMock
            .Setup(repo => repo.GetByUserIdAsync(userId))
            .ReturnsAsync(allCollections
                .Where(collection => collection.UserId == userId)
                .ToList());

        var service = CreateService();
        
        // Act
        var result = await service.GetMyCollectionsAsync(userId);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(collection => collection.UserId == userId);
        result.Should().NotContain(collection => collection.UserId == 2);
        
        _collectionRepositoryMock.Verify(
            repo => repo.GetByUserIdAsync(userId),
            Times.Once);
    }

    // Use case: UC03 - Persoonlijke collectie bekijken
    // Testcase: UTC-UC03-02
    // Doel: Controleren of een gebruiker geen privécollectie van een andere gamer kan ophalen
    [Fact]
    public async Task UTC_UC03_02_GetByIdAsync_ShouldReturnNull_WhenCollectionBelongsToAnotherUser()
    {
        // Arrange
        var loggedInUserId = 1; // Gamer A
        var gamerB = 2;
        var collectionId = 10;

        var collectionFromOtherUser = new Collection
        {
            Id = collectionId,
            Name = "Privécollectie van gamer B",
            Description = "Deze collectie is niet van gamer A",
            UserId = gamerB,
            IsPublic = false
        };

        _collectionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(collectionId))
            .ReturnsAsync(collectionFromOtherUser);

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(collectionId, loggedInUserId);

        // Assert
        result.Should().BeNull();

        _collectionRepositoryMock.Verify(
            repo => repo.GetByIdAsync(collectionId),
            Times.Once
        );
    }
    
    // Use case: UC12 - Persoonlijke collectie verwijderen
    // Testcase: UTC-UC12-01
    // Doel: Controleren of een ingelogde gebruiker een eigen persoonlijke collectie kan verwijderen.
    [Fact]
    public async Task UTC_UC12_01_DeleteAsync_ShouldDeleteCollection_WhenCollectionBelongsToUser()
    {
        // Arrange
        const int collectionId = 1;
        const int userId = 1;

        var collection = new Collection
        {
            Id = collectionId,
            Name = "Collectie van Gamer A",
            Description = "Eigen persoonlijke collectie",
            UserId = userId,
            IsPublic = false
        };

        _collectionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(collectionId))
            .ReturnsAsync(collection);

        _collectionRepositoryMock
            .Setup(repo => repo.DeleteAsync(collectionId))
            .ReturnsAsync(true);
        
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(collectionId, userId);

        // Assert
        result.Should().BeTrue();
        
        _collectionRepositoryMock.Verify(
            repo => repo.GetByIdAsync(collectionId),
            Times.Once
        );

        _collectionRepositoryMock.Verify(
            repo => repo.DeleteAsync(collectionId),
            Times.Once
        );
    }
    
    // Use case: UC12 - Persoonlijke collectie verwijderen
    // Testcase: UTC-UC12-02
    // Doel: Controleren of een gebruiker geen collectie kan verwijderen waarvan die geen eigenaar is.
    [Fact]
    public async Task UTC_UC12_02_DeleteAsync_ShouldReturnFalse_WhenCollectionBelongsToAnotherUser()
    {
        // Arrange
        const int collectionId = 1;
        const int gamerAUserId = 1;
        const int gamerBUserId = 2;
        
        var collectionFromGamerA = new Collection
        {
            Id = collectionId,
            Name = "Collectie van Gamer A",
            Description = "Eigen persoonlijke collectie van Gamer A",
            UserId = gamerAUserId,
            IsPublic = false
        };
        
        _collectionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(collectionId))
            .ReturnsAsync(collectionFromGamerA);
        
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(collectionId, gamerBUserId);

        // Assert
        result.Should().BeFalse();

        _collectionRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<int>()),
            Times.Once
        );

        _collectionRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<int>()),
            Times.Never);
    }
    
    // Use case: UC12 - Persoonlijke collectie verwijderen
    // Testcase: UTC-UC12-03
    // Doel: Controleren of verwijderen van een niet-bestaande collectie een foutresultaat geeft.
    [Fact]
    public async Task UTC_UC12_03_DeleteAsync_ShouldReturnFalse_WhenCollectionDoesNotExist()
    {
        // Arrange
        const int collectionId = 999;
        const int userId = 1;

        _collectionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(collectionId))
            .ReturnsAsync((Collection?)null);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(collectionId, userId);

        // Assert
        result.Should().BeFalse();

        _collectionRepositoryMock.Verify(
            repo => repo.GetByIdAsync(collectionId),
            Times.Once
        );

        _collectionRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<int>()),
            Times.Never
        );
    }
    
    // Use case: UC12 - Persoonlijke collectie verwijderen
    // Testcase: UTC-UC12-04
    // Doel: Controleren of een collectie niet verwijderd kan worden wanneer er geen gebruiker bekend is.
    [Fact]
    public async Task UTC_UC12_04_DeleteAsync_ShouldReturnFalse_WhenUserIdIsNull()
    {
        // Arrange
        const int collectionId = 1;

        var collection = new Collection
        {
            Id = collectionId,
            Name = "Collectie van Gamer A",
            Description = "Collectie waarvoor geen geldige gebruiker is meegegeven",
            UserId = 1,
            IsPublic = false
        };

        _collectionRepositoryMock
            .Setup(repo => repo.GetByIdAsync(collectionId))
            .ReturnsAsync(collection);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(collectionId, null);

        // Assert
        result.Should().BeFalse();

        _collectionRepositoryMock.Verify(
            repo => repo.GetByIdAsync(collectionId),
            Times.Once
        );

        _collectionRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<int>()),
            Times.Never
        );
    }
}