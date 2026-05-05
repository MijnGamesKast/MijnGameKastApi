using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController : CustomBaseController
{
    private readonly ICollectionService _collectionService;
    private readonly ICollectionGameService _collectionGameService;

    public CollectionController(ICollectionService collectionService, ICollectionGameService collectionGameService)
    {
        _collectionService = collectionService;
        _collectionGameService = collectionGameService;
    }
    
    [RequireAuth]
    [HttpGet]
    public async Task<IActionResult> GetCollection()
    {
        var userId = GetUserId();

        var collections = await _collectionService.GetMyCollectionsAsync(userId);
        
        return Ok(collections);
    }

    [RequireAuth]
    [HttpGet("{collectionId}")]
    public async Task<IActionResult> GetSpecificCollection(int collectionId)
    {
        var userId = GetUserId();

        var collection = await _collectionService.GetByIdAsync(collectionId, userId);
        if (collection == null)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden of je hebt geen toegang tot deze collectie"
            });
        }
        
        return Ok(collection);
    }

    [RequireAuth]
    [HttpPost]
    public async Task<IActionResult> CreateCollection([FromBody] Collection collection)
    {
        Console.WriteLine("Create Collection Test 1");
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Create Collection Test 2");
            return BadRequest(new
            {
                Message = "De ingevoerde gegevens zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }
        Console.WriteLine("Create Collection Test 3");
        
        var userId = GetUserId();

        Console.WriteLine("Create Collection Test 4");
        var createdCollection = await _collectionService.CreateAsync(collection, userId);
        
        Console.WriteLine("Create Collection Test 5");
        return CreatedAtAction(nameof(GetSpecificCollection), new { collectionId = createdCollection.Id }, createdCollection);
    }

    [RequireAuth]
    [HttpPut("{collectionId}")]
    public async Task<IActionResult> UpdateCollection(int collectionId, [FromBody] Collection collection)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De ingevoerde gegevens zijn ongeldig.",
                Errors = GetValidationErrors()
            });
        }
        
        var userId = GetUserId();
        
        var updatedCollection = await _collectionService.UpdateAsync(collectionId, collection, userId);

        if (!updatedCollection)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden of is niet van jou."
            });
        }

        return Ok(new
        {
            Message = "Collectie is succesvol gewijzigd",
        });
    }

    [RequireAuth]
    [HttpDelete("{collectionId}")]
    public async Task<IActionResult> DeleteCollection(int collectionId)
    {
        var userId = GetUserId();
        var deleted = await _collectionService.DeleteAsync(collectionId, userId);

        if (!deleted)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden of is niet van jou"
            });
        }

        return Ok(new
        {
            Message = $"Collectie met id {collectionId} is succesvol verwijderd."
        });
    }

    [RequireAuth]
    [HttpGet("{collectionId}/games")]
    public async Task<IActionResult> GetGamesInCollection(int collectionId)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven."
            });
        }

        var games = await _collectionGameService.GetGamesByCollectionIdAsync(collectionId, token);

        if (games == null)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden, is niet van jou, of je sessie is ongeldig/verlopen."
            });
        }

        return Ok(games);
    }

    [RequireAuth]
    [HttpPost("{collectionId}/games/{gameId}")]
    public async Task<IActionResult> AddGameToCollection(int collectionId, int gameId)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven."
            });
        }

        var added = await _collectionGameService.AddGameToCollectionAsync(collectionId, gameId, token);

        if (!added)
        {
            return BadRequest(new
            {
                Message = $"Game met id {gameId} kon niet aan collectie ({collectionId}) worden toegevoegd"
            });
        }

        return Ok(new
        {
            Message = $"Game met id {gameId} is succesvol toegevoegd aan collectie ({collectionId})"
        });
    }

    [RequireAuth]
    [HttpDelete("{collectionId}/games/{gameId}")]
    public async Task<IActionResult> RemoveGameFromCollection(int collectionId, int gameId)
    {
        var token = GetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new
            {
                Message = "Er is geen token meegegeven."
            });
        }

        var removed = await _collectionGameService.RemoveGameFromCollectionAsync(collectionId, gameId, token);

        if (!removed)
        {
            return NotFound(new
            {
                Message = $"Game me id {gameId} is niet gevonden in collectie ({collectionId})"
            });
        }

        return Ok(new
        {
            Message = $"Game met Id {gameId} is succesvol verwijderd uit de collectie ({collectionId})"
        });
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

    
}