using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
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
        return Ok(await _catalogService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _catalogService.GetByIdAsync(id);

        if (game == null)
        {
            return NotFound(new
            {
                Message = $"Game met id ({id}) is niet gevonden!"
            });
        }

        return Ok(game);
    }

    [RequireAuth]
    [HttpPost]
    public async Task<IActionResult> AddGame([FromBody] CreateGameRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
                {
                    Message = "De ingevoerde gegevens zijn ongeldig!",
                    Errors = GetValidationErrors()
                });
        }

        var userId = GetUserId();

        var createdGame = await _catalogService.AddGameAsync(request, userId);

        return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] UpdateGameRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De ingevoerde gegevens zijn ongeldig!",
                Errors = GetValidationErrors()
            });
        }
        
        var result = await _catalogService.UpdateGameAsync(id, request);
        return ToActionResult(result);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var result = await _catalogService.DeleteGameAsync(id);
        return ToActionResult(result);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> ApproveGame(int id)
    {
        var result = await _catalogService.ApproveGameAsync(id);
        return ToActionResult(result);
    }


    [RequireAuth(ModeratorOnly = true)]
    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> RejectGame(int id)
    {
        var result = await _catalogService.RejectGameAsync(id);
        return ToActionResult(result);
    }
}