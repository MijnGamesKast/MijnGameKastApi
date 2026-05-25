using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MijnGameKast.API.Controllers;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Tests.Controllers;

public class CatalogControllerTests
{
    private readonly Mock<ICatalogService> _catalogServiceMock = new();

    private CatalogController CreateController()
    {
        return new CatalogController(_catalogServiceMock.Object);
    }
    
    // Use case: UC16 - Catalogusgame wijzigen
    // Testcase: UTC-UC16-03
    // Doel: Controleren of een catalogusgame niet wordt gewijzigd wanneer de nieuwe gegevens ongeldig zijn.
    [Fact]
    public async Task UTC_UC16_03_UpdateGame_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        const int gameId = 1;

        var request = new UpdateGameRequest
        {
            Title = string.Empty,
            Description = "Beschrijving"
        };

        var controller = CreateController();

        // In een echte API-request doet ASP.NET model validation automatisch.
        // In een unit test moeten we ModelState handmatig ongeldig maken.
        controller.ModelState.AddModelError(
            nameof(UpdateGameRequest.Title),
            "Vul de titel in."
        );

        // Act
        var result = await controller.UpdateGame(gameId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _catalogServiceMock.Verify(
            service => service.UpdateGameAsync(It.IsAny<int>(), It.IsAny<UpdateGameRequest>()),
            Times.Never
        );
    }
}