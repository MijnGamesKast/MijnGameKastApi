using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Attributes;
using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformController : CustomBaseController
{
    private readonly IPlatformService _platformService;

    public PlatformController(IPlatformService platformService)
    {
        _platformService = platformService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var platforms = await _platformService.GetAllAsync();
        return Ok(platforms);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetbyId(int id)
    {
        var platform = await _platformService.GetByIdAsync(id);
        if (platform == null)
        {
            return NoContent();
        }

        return Ok(platform);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPost]
    public async Task<IActionResult> AddPlatform([FromBody] Platform platform)
    {
        if (!ModelState.IsValid) // check if the moddel is valid
        {
            return BadRequest(new
            {
                Message = "De platformgegevens zijn ongeldig",
                Errors = GetValidationErrors()
            });
        }

        var createdPlatform = await _platformService.AddPlatform(platform);
        return CreatedAtAction(nameof(GetbyId), new { id = createdPlatform.Id }, createdPlatform);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlatform(int id, [FromBody] Platform platform)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "De platformgegevens zijn ongeldig",
                Error = GetValidationErrors()
            });
        }

        var result = await _platformService.UpdatePlatformAsync(id, platform);
        return ToActionResult(result);
    }

    [RequireAuth(ModeratorOnly = true)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatform(int id)
    {
        var result = await _platformService.DeletePlatformAsync(id);
        return ToActionResult(result);
    }
}