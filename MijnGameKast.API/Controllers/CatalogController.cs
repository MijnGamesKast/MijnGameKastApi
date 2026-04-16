using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _catalogService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _catalogService.GetByIdAsync(id);
        if (game == null)
        {
            return NoContent();
        }

        return Ok(game);
    }

    [HttpPost]
    public async Task<IActionResult> AddGame([FromBody] Game game)
    {
        // Check if the game is valid for the Model
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    field = x.Key,
                    errors = x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();
            
            return BadRequest(new
            {
                message = "De opgegeven game is ongeldig.",
                validationErrors = errors
            });
        }

        var createdGame = await _catalogService.AddGameAsync(game);

        return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    field = x.Key,
                    errors = x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();
            
            return BadRequest(new
            {
                message = "De opgegeven game is ongeldig.",
                validationErrors = errors
            });
        }
        
        var updatedGame = await _catalogService.UpdateGameAsync(id, game);

        if (!updatedGame)
        {
            return NotFound(new
            {
                message = $"Game met id {id} is niet gevonden!"
            });
        }
        
        return Ok(new
        {
            message = $"Game met id {id} is succesvol gewijzigd!"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var deletedGame = await _catalogService.DeleteGameAsync(id);

        if (!deletedGame)
        {
            return NotFound(new
            {
                message = $"Game met {id} is niet gevonden!"
            });
        }

        return Ok(new
        {
            message = $"Game met id {id} is succesvol verwijderd!"
        });
    }
}