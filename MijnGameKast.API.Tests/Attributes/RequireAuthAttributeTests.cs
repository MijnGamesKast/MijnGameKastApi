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
    private static ActionExecutingContext CreateActionExecutingContext(
        IServiceProvider serviceProvider,
        string? token = null)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        
        if (token != null)
        {
            httpContext.Items["Token"] = token;
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ControllerActionDescriptor()
        );

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object()
        );
    }

    private static ActionExecutionDelegate CreateNextDelegate(
        ActionContext actionContext,
        Action<bool> setActionWasExecuted)
    {
        return () =>
        {
            setActionWasExecuted(true);

            return Task.FromResult(new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                controller: new object()
            ));
        };
    }

    private static ServiceProvider CreateServiceProvider(
        Mock<ISessionRepository> sessionRepositoryMock,
        Mock<IUserRepository> userRepositoryMock)
    {
        var services = new ServiceCollection();

        services.AddSingleton(sessionRepositoryMock.Object);
        services.AddSingleton(userRepositoryMock.Object);

        return services.BuildServiceProvider();
    }
    
    
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
    
    // Non-functional requirement: NFR06 - Beveiligde endpoints controleren sessie/token
    // Testcase: UTC-NFR06-01
    // Doel: Controleren of een geldige token toegang geeft tot een beveiligde actie.
    [Fact]
    public async Task UTC_NFR06_01_RequireAuthAttribute_ShouldAllowAction_WhenTokenIsValid()
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

        var user = new User
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
            .ReturnsAsync(user);

        var serviceProvider = CreateServiceProvider(
            sessionRepositoryMock,
            userRepositoryMock
        );

        var executingContext = CreateActionExecutingContext(
            serviceProvider,
            token
        );

        var actionWasExecuted = false;

        var next = CreateNextDelegate(
            executingContext,
            value => actionWasExecuted = value
        );

        var attribute = new RequireAuthAttribute();

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeTrue();

        executingContext.Result.Should().BeNull();

        executingContext.HttpContext.Items["UserId"].Should().Be(session.UserId);
        executingContext.HttpContext.Items["User"].Should().Be(user);
        executingContext.HttpContext.Items["Session"].Should().Be(session);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(token),
            Times.Once
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(session.UserId),
            Times.Once
        );
    }

    // Non-functional requirement: NFR06 - Beveiligde endpoints controleren sessie/token
    // Testcase: UTC-NFR06-02
    // Doel: Controleren of een beveiligde actie wordt geweigerd wanneer er geen token aanwezig is.
    [Fact]
    public async Task UTC_NFR06_02_RequireAuthAttribute_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        // Arrange
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        var serviceProvider = CreateServiceProvider(
            sessionRepositoryMock,
            userRepositoryMock
        );

        // Geen token meegeven.
        var executingContext = CreateActionExecutingContext(serviceProvider);

        var actionWasExecuted = false;

        var next = CreateNextDelegate(
            executingContext,
            value => actionWasExecuted = value
        );

        var attribute = new RequireAuthAttribute();

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

    // Non-functional requirement: NFR06 - Beveiligde endpoints controleren sessie/token
    // Testcase: UTC-NFR06-03
    // Doel: Controleren of een beveiligde actie wordt geweigerd wanneer de token niet bij een bestaande sessie hoort.
    [Fact]
    public async Task UTC_NFR06_03_RequireAuthAttribute_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        const string token = "invalid-token";

        var sessionRepositoryMock = new Mock<ISessionRepository>();
        sessionRepositoryMock
            .Setup(repo => repo.GetByTokenAsync(token))
            .ReturnsAsync((Session?)null);

        var userRepositoryMock = new Mock<IUserRepository>();

        var serviceProvider = CreateServiceProvider(
            sessionRepositoryMock,
            userRepositoryMock
        );

        var executingContext = CreateActionExecutingContext(
            serviceProvider,
            token
        );

        var actionWasExecuted = false;

        var next = CreateNextDelegate(
            executingContext,
            value => actionWasExecuted = value
        );

        var attribute = new RequireAuthAttribute();

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(401);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(token),
            Times.Once
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<int?>()),
            Times.Never
        );
    }

    // Non-functional requirement: NFR06 - Beveiligde endpoints controleren sessie/token
    // Testcase: UTC-NFR06-04
    // Doel: Controleren of een beveiligde actie wordt geweigerd wanneer de sessie/token verlopen is.
    [Fact]
    public async Task UTC_NFR06_04_RequireAuthAttribute_ShouldReturnUnauthorized_WhenSessionIsExpired()
    {
        // Arrange
        const string token = "expired-token";

        var expiredSession = new Session
        {
            Id = 1,
            Token = token,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddHours(-7),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        };

        var sessionRepositoryMock = new Mock<ISessionRepository>();
        sessionRepositoryMock
            .Setup(repo => repo.GetByTokenAsync(token))
            .ReturnsAsync(expiredSession);

        var userRepositoryMock = new Mock<IUserRepository>();

        var serviceProvider = CreateServiceProvider(
            sessionRepositoryMock,
            userRepositoryMock
        );

        var executingContext = CreateActionExecutingContext(
            serviceProvider,
            token
        );

        var actionWasExecuted = false;

        var next = CreateNextDelegate(
            executingContext,
            value => actionWasExecuted = value
        );

        var attribute = new RequireAuthAttribute();

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(401);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(token),
            Times.Once
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<int?>()),
            Times.Never
        );
    }

    // Non-functional requirement: NFR06 - Beveiligde endpoints controleren sessie/token
    // Testcase: UTC-NFR06-05
    // Doel: Controleren of een beveiligde actie wordt geweigerd wanneer de sessie verwijst naar een gebruiker die niet bestaat.
    [Fact]
    public async Task UTC_NFR06_05_RequireAuthAttribute_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        const string token = "valid-token-with-missing-user";

        var session = new Session
        {
            Id = 1,
            Token = token,
            UserId = 999,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var sessionRepositoryMock = new Mock<ISessionRepository>();
        sessionRepositoryMock
            .Setup(repo => repo.GetByTokenAsync(token))
            .ReturnsAsync(session);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(session.UserId))
            .ReturnsAsync((User?)null);

        var serviceProvider = CreateServiceProvider(
            sessionRepositoryMock,
            userRepositoryMock
        );

        var executingContext = CreateActionExecutingContext(
            serviceProvider,
            token
        );

        var actionWasExecuted = false;

        var next = CreateNextDelegate(
            executingContext,
            value => actionWasExecuted = value
        );

        var attribute = new RequireAuthAttribute();

        // Act
        await attribute.OnActionExecutionAsync(executingContext, next);

        // Assert
        actionWasExecuted.Should().BeFalse();

        executingContext.Result.Should().BeOfType<UnauthorizedObjectResult>();

        var result = (UnauthorizedObjectResult)executingContext.Result!;
        result.StatusCode.Should().Be(401);

        sessionRepositoryMock.Verify(
            repo => repo.GetByTokenAsync(token),
            Times.Once
        );

        userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(session.UserId),
            Times.Once
        );
    }
}