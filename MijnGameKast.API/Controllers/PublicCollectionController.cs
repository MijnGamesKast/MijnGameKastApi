using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Controllers;

[ApiController]
[Route("api/public/collection")]
public class PublicCollectionController : CustomBaseController
{
    private readonly IPublicCollectionService _publicCollectionService;

    public PublicCollectionController(IPublicCollectionService publicCollectionService)
    {
        _publicCollectionService = publicCollectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPublicCollections()
    {
        return Ok(await _publicCollectionService.GetPublicCollectionsAsync());
    }

    [HttpGet("{collectionId}")]
    public async Task<IActionResult> GetSpecificPublicCollection(int collectionId)
    {
        var result = await _publicCollectionService.GetPublicCollectionByIdAsync(collectionId);
        if (result == null)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden"
            });
        }
        
        return Ok(result);
    }

    [HttpGet("{collectionId}/games")]
    public async Task<IActionResult> GetGamesInPublicCollection(int collectionId)
    {
        var result = await _publicCollectionService.GetPublicCollectionGamesAsync(collectionId);
        if (result == null)
        {
            return NotFound(new
            {
                Message = $"Collectie met id {collectionId} is niet gevonden"
            });
        }
        
        return Ok(result);
    }
}