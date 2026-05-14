using System.Net;
using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Controllers;

public class CustomBaseController : ControllerBase
{
    protected IActionResult ToActionResult(ServiceResult result)
    {
        return result.Type switch
        {
            ServiceResultType.Success => Ok(result),
            ServiceResultType.NotFound => NotFound(result),
            ServiceResultType.Unauthorized => Unauthorized(result),
            ServiceResultType.Forbidden => StatusCode(403, result),
            ServiceResultType.BadRequest => BadRequest(result),
            ServiceResultType.Conflict => Conflict(result),
            _ => StatusCode(500, result)
        };
    }

    protected Dictionary<string, List<string>> GetValidationErrors()
    {
        return ModelState
            .Where(x => x.Value is not null && x.Value.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }

    protected string? GetToken()
    {
        return HttpContext.Items["Token"] as string;
    }

    protected int? GetUserId()
    {
        return HttpContext.Items["UserId"] as int?;
    }

    protected User? GetUser()
    {
        return HttpContext.Items["User"] as User;
    }

    protected Session? GetSession()
    {
        return HttpContext.Items["Session"] as Session;
    }
}