using FluentAssertions;
using Moq;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Auth;
using MijnGameKast.API.Services;

namespace MijnGameKast.API.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();

    private AuthService CreateService()
    {
        return new AuthService(
            _userRepositoryMock.Object,
            _sessionRepositoryMock.Object
        );
    }

    // Use case: UC07 - Inloggen
    // Testcase: UTC-UC07-01
    // Doel: Controleren of een gebruiker met geldige inloggegevens succesvol kan inloggen en een sessie/token krijgt.
    [Theory]
    [InlineData("gamer@mijngamekast.nl", true)]
    [InlineData("gamer", false)]
    public async Task UTC_UC07_01_LoginAsync_ShouldCreateSessionAndReturnToken_WhenCredentialsAreValide
        (string identifier, bool isEmailLogin)
    {
        // Arrange
        const string password = "Welkom123!";

        var user = new User
        {
            Id = 1,
            Username = "Gamer",
            Email = "gamer@mijngamekast.nl",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            Identifier = identifier,
            Password = password
        };

        if (isEmailLogin)
        {
            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(identifier))
                .ReturnsAsync(user);
        }
        else
        {
            _userRepositoryMock
                .Setup(repo => repo.GetByUsernameAsync(identifier))
                .ReturnsAsync(user);
        }

        _sessionRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session session) => session);
        
        var service = CreateService();
        
        // Act
        var result = await service.LoginAsync(request);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.UserId.Should().Be(user.Id);
        
        _sessionRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Session>()),
            Times.Once
        );
    }

    // Use case: UC07 - Inloggen
    // Testcase: UTC-UC07-02
    // Doel: Controleren of een gebruiker niet kan inloggen met een verkeerd wachtwoord.
    [Fact]
    public async Task UTC_UC07_02_LoginAsync_ShouldRejectLogin_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "gamer",
            Email = "gamer@mijngamekast.nl",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            CreatedAt = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            Identifier = "gamer@mijngamekast.nl",
            Password = "WrongPassword123!"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(request.Identifier))
            .ReturnsAsync(user);

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Message.Should().Be("Ongeldige inloggegevens!");

        _sessionRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Session>()),
            Times.Never
        );
    }

    // Use case: UC07 - Inloggen
    // Testcase: UTC-UC07-03
    // Doel: Controleren of een loginpoging met een niet-bestaand account wordt geweigerd.
    [Fact]
    public async Task UTC_UC07_03_LoginAsync_ShouldRejectLogin_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginRequest
        {
            Identifier = "nietbestaand@mijngamekast.nl",
            Password = "Willekeurig123!"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(request.Identifier))
            .ReturnsAsync((User?)null);

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNullOrEmpty();
        result.Message.Should().Be("Ongeldige inloggegevens!");

        _sessionRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Session>()),
            Times.Never
        );
    }
}