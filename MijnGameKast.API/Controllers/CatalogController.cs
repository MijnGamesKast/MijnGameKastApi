using MijnGameKast.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private static List<Game> _games = new List<Game>()
    {
        new Game() { Id = 1, Title = "Grand theft auto V", Description = ""},
        new Game() { Id = 2, Title = "Bloodborne", Description = ""},
        new Game() { Id = 3, Title = "Harry Potter 2", Description = "", Platforms = ["Playstation 1", "Playstation 2", "PC", "Gameboy Color", "Gameboy Advance", "Xbox", "Nintendo Gamecube"]}
    };

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_games);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Game? game = _games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            return NoContent();
        }
        
        
        return Ok(game);
    }

    [HttpPost]
    // [ValidateAntiForgeryToken]
    public IActionResult Post([FromForm] Game game)
    {
        Console.WriteLine("Post method");
        Console.WriteLine($"Model {ModelState}");
        Console.WriteLine($"ModelStateValid {ModelState.IsValid}");

        
        if (!ModelState.IsValid)
        {
            // Model is not valid and not correct
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Field: {state.Key} - Error: {error.ErrorMessage}");
                }
            }

            return NotFound("Not all fields are filled in correctly!");
        }

        // Check if the id is unique
        Game? gameCheck = _games.FirstOrDefault(g => g.Id == game.Id);
        if (gameCheck != null)
        {
            // Game already exists
            return BadRequest("Er is al een game toegevoegd met dit Id, probeer het opnieuw met een ander Id. Wel bedankt voor het gebruik maken van ons systeem!");
        }
        
        Game newGame = new Game() { Id = game.Id, Title = game.Title, Description = game.Description };
        _games.Add(newGame);        
        return CreatedAtAction(
            nameof(GetById),
            new { id = newGame.Id },
            newGame
            );
    }
}