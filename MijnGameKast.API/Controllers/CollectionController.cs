using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCollection()
    {
        return Ok("Collection");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecificCollection(int id)
    {
        return Ok("Specific Collection");
    }

    [HttpPost]
    public async Task<IActionResult> CreateCollection()
    {
        return Ok("Collection added");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCollection()
    {
        return Ok("Collection updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        return Ok("Collection deleted");
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
}