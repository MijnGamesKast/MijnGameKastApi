using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : CustomBaseController
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

    [RequireAuth]
    [HttpPost]
    public async Task<IActionResult> AddGame([FromBody] Game game)
    {
        // Add the extra info to the game (user that added it and everything)
        var userId = GetUserId();
        game.UserId = userId ?? null;
        
        // Check if the game is valid for the Model
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De gamegegevens zijn ongeldig",
                Errors = GetValidationErrors()
            });
        }
        
        var createdGame = await _catalogService.AddGameAsync(game);

        return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
    {
        // Add the user to the game model
        var userId = GetUserId();
        game.UserId = userId;
        
        if (!ModelState.IsValid) // Check if the model is made correct
        {
            return BadRequest(new
            {
                Message = "De gamegegevens zijn ongeldig",
                Errors = GetValidationErrors()
            });
        }
        
        var result = await _catalogService.UpdateGameAsync(id, game);

        return ToActionResult(result);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        int? userId = GetUserId();
        var result = await _catalogService.DeleteGameAsync(id, userId);
        return ToActionResult(result);
    }
}