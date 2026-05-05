namespace MijnGameKast.API.Middleware;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;
    
    public TokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader["Bearer ".Length..].Trim();

            context.Items["Token"] = token;
        }
        
        await _next(context); // Call the next delegate (controller)
    }
}