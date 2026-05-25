using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MijnGameKast.API.Controllers;
using MijnGameKast.API.Data.Models.Auth;
using MijnGameKast.API.Services.Interfaces;


namespace MijnGameKast.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    private AuthController CreateController()
    {
        return new AuthController(_authServiceMock.Object);
    }

    // Use case: UC07 - Inloggen
    // Testcase: UTC-UC07-04
    // Doel: Controleren of een loginpoging met lege invoervelden wordt geweigerd.
    [Fact]
    public async Task UTC_UC07_04_Login_ShouldReturnBadRequest_WhenLoginValuesAreEmpty()
    {
        // Arrange
        var request = new LoginRequest
        {
            Identifier = string.Empty,
            Password = string.Empty
        };

        var controller = CreateController();

        controller.ModelState.AddModelError(nameof(LoginRequest.Identifier), "Vul een gebruikersnaam of e-mailadres in.");
        controller.ModelState.AddModelError(nameof(LoginRequest.Password), "Vul een wachtwoord in.");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _authServiceMock.Verify(
            service => service.LoginAsync(It.IsAny<LoginRequest>()),
            Times.Never
        );
    }
    
    // Use case: UC07 - Inloggen
    // Testcase: UTC-UC07-05
    // Doel: Controleren of een loginpoging zonder geldige requestdata wordt geweigerd.
    [Fact]
    public async Task UTC_UC07_05_Login_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.Login(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _authServiceMock.Verify(
            service => service.LoginAsync(It.IsAny<LoginRequest>()),
            Times.Never
        );
    }
}