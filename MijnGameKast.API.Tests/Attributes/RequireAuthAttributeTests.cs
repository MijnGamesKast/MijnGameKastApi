using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Tests.Attributes;

public class RequireAuthAttributeTests
{
    // Use case: UC08 - Catalogusgame verwijderen
    // Testcase: UTC-UC08-02
    // Doel: Controleren of een gebruiker zonder moderatorrol geen catalogusgame kan verwijderen
    [Fact]
    public async Task UTC_UC08_02_RequireAuthAttribute_ShouldReturnForbidden_WhenUserIsNotModerator()
    {
        // Arrange
        const string token = "validtoken";

        var session = new Session
        {
            Id = 1,
            Token = token,
            UserId = 1,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var gamer = new User
        {
            Id = 1,
            Username = "gamer",
            Email = "gamer@mijngamekast.nl",
            Role = UserRole.Gamer
        };

        var sessionRepositoryMock = new Mock<ISessionRepository>();
        sessionRepositoryMock
            .Setup(repo => repo.GetByTokenAsync(token))
            .ReturnsAsync(session);
        
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(session.UserId))
            .ReturnsAsync(gamer);

        var services = new ServiceCollection();
        services.AddSingleton(sessionRepositoryMock.Object);
        services.AddSingleton(userRepositoryMock.Object);
        
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        httpContext.Items["Token"] = token;

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object()
        );

        var actionWasExecuted = false;

        ActionExecutionDelegate next = () =>
        {
            actionWasExecuted = true;

            return Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller: new object()
            ));
        };

        var attribute = new RequireAuthAttribute
        {
            ModeratorOnly = true
        };
        
        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(403);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(token),
            Times.Once
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(session.UserId),
            Times.Once
        );
    }
    
    // Use case: UC16 - Catalogusgame wijzigen
    // Testcase: UTC-UC16-02
    // Doel: Controleren of een normale gamer geen moderator-only wijzigactie mag uitvoeren.
    [Fact]
    public async Task UTC_UC16_02_RequireAuthAttribute_ShouldReturnForbidden_WhenUserIsNotModerator()
    {
        // Arrange
        const string token = "valid-token";

        var session = new Session
        {
            Id = 1,
            Token = token,
            UserId = 1,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var gamer = new User
        {
            Id = 1,
            Username = "gamer",
            Email = "gamer@mijngamekast.nl",
            Role = UserRole.Gamer
        };

        var sessionRepositoryMock = new Mock<ISessionRepository>();
        sessionRepositoryMock
            .Setup(repo => repo.GetByTokenAsync(token))
            .ReturnsAsync(session);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(session.UserId))
            .ReturnsAsync(gamer);

        var services = new ServiceCollection();
        services.AddSingleton(sessionRepositoryMock.Object);
        services.AddSingleton(userRepositoryMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        // Normaal doet TokenMiddleware dit. In deze unit test doen we het handmatig.
        httpContext.Items["Token"] = token;

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object()
        );

        var actionWasExecuted = false;

        ActionExecutionDelegate next = () =>
        {
            actionWasExecuted = true;

            return Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller: new object()
            ));
        };

        var attribute = new RequireAuthAttribute
        {
            ModeratorOnly = true
        };

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(403);
    }
    
    // Use case: UC16 - Catalogusgame wijzigen
    // Testcase: UTC-UC16-05
    // Doel: Controleren of een wijzigactie zonder sessie/token wordt geweigerd.
    [Fact]
    public async Task UTC_UC16_05_RequireAuthAttribute_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        var services = new ServiceCollection();
        services.AddSingleton(sessionRepositoryMock.Object);
        services.AddSingleton(userRepositoryMock.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        // Belangrijk:
        // We zetten bewust GEEN httpContext.Items["Token"].
        // Daarmee simuleren we een request zonder Authorization/Bearer token.

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object()
        );

        var actionWasExecuted = false;

        ActionExecutionDelegate next = () =>
        {
            actionWasExecuted = true;

            return Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller: new object()
            ));
        };

        var attribute = new RequireAuthAttribute
        {
            ModeratorOnly = true
        };

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(401);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(It.IsAny<string>()),
            Times.Never
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<int?>()),
            Times.Never
        );
    }
}