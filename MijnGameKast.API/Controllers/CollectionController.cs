using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCollection()
    {
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }

        var collections = await _collectionService.GetMyCollectionsAsync(token);

        if (collections == null)
        {
            return Unauthorized(new
            {
                Message = "Geen geldige sessie gevonden!"
            });
        }
        
        return Ok(collections);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificCollection(int id)
    {
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }

        var collection = await _collectionService.GetByIdAsync(id, token);
        if (collection == null)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {id} is niet gevonden of je hebt geen toegang tot deze collectie"
            });
        }
        
        return Ok(collection);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCollection([FromBody] Collection collection)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De ingevoerde gegevens zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }
        
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }

        var createdCollection = await _collectionService.CreateAsync(collection, token);

        if (createdCollection == null)
        {
            return Unauthorized(new
            {
                Message = "Geen geldige sessie gevonden!"
            });
        }
        
        return CreatedAtAction(nameof(GetSpecificCollection), new { id = createdCollection.Id }, createdCollection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCollection(int id, [FromBody] Collection collection)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De ingevoerde gegevens zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }

        var token = GetBearerToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven"
            });
        }
        
        var updatedCollection = await _collectionService.UpdateAsync(id, collection, token);

        if (!updatedCollection)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {id} is niet gevonden, is niet van jou, of je sessie is ongeldig."
            });
        }

        return Ok(new
        {
            Message = "Collectie is succesvol gewijzigd",
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var token = GetBearerToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven."
            });
        }

        var deleted = await _collectionService.DeleteAsync(id, token);

        if (!deleted)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {id} is niet gevonden, is niet van jou, of je sessie is ongeldig."
            });
        }

        return Ok(new
        {
            Message = $"Collectie met id {id} is succesvol verwijderd."
        });
    }

    [HttpGet("{id}/games")]
    public async Task<IActionResult> GetGamesInCollection(int id)
    {
        return Ok("Games in collection");
    }

    [HttpPost("{id}/games/{gameId}")]
    public async Task<IActionResult> AddGameToCollection(int id, int gameId)
    {
        return Ok("Game added to collection");
    }

    [HttpDelete("{id}/games/{gameId}")]
    public async Task<IActionResult> RemoveGameFromCollection(int id, int gameId)
    {
        return Ok("Game removed from collection");
    }
    
    private Dictionary<string, List<string>> GetValidationErrors()
    {
        return ModelState
            .Where(x => x.Value is not null && x.Value.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }

    private string? GetBearerToken()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";

        if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return authorizationHeader[bearerPrefix.Length..].Trim();
    }
}