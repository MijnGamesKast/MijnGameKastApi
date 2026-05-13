using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : CustomBaseController
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }
    
    [RequireAuth(ModeratorOnly = true)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var genres = await _genreService.GetAllAsync();
        return Ok(genres);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        if (genre == null)
        {
            return NoContent();
        }

        return Ok(genre);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPost]
    public async Task<IActionResult> AddGenre([FromBody] Genre genre)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De genregegevens zijn ongeldig",
                Error = GetValidationErrors()
            });
        }

        var createdGenre = await _genreService.AddGenre(genre);
        return CreatedAtAction(nameof(GetById), new { id = createdGenre.Id }, createdGenre);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var result = await _genreService.DeleteGenreAsync(id);
        return ToActionResult(result);
    }
}