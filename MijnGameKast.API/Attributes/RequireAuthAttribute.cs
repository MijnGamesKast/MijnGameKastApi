using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Attributes;

public class RequireAuthAttribute : Attribute, IAsyncActionFilter
{
    public bool ModeratorOnly { get; set; } = false;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sessionRepository = context.HttpContext.RequestServices.GetRequiredService<ISessionRepository>();
        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        var token = context.HttpContext.Items["Token"] as string;

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Er is geen token meegegeven"
            });
            return;
        }

        var session = await sessionRepository.GetByTokenAsync(token);

        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Ongeldige of verlopen sessie"
            });
            return;
        }

        var user = await userRepository.GetByIdAsync(session.UserId);
        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Gebruiker bestaat niet"
            });
            return;
        }
        
        // Check if a user has the required role
        if (ModeratorOnly && user.Role != UserRole.Moderator)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Gebruiker heeft niet de juiste rechten"
            })
            {
                StatusCode = 403
            };
            return;
        }
        
        context.HttpContext.Items["UserId"] = session.UserId;
        context.HttpContext.Items["User"] = user;
        context.HttpContext.Items["Session"] = session;

        await next();
    }
    
}