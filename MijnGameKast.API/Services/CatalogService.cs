using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Services.Interfaces;

namespace MijnGameKast.API.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogRepository _catalogRepository;

    public CatalogService(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<List<Game>> GetAllAsync()
    {
        return _catalogRepository.GetAllAsync();
    }

    public Task<Game?> GetByIdAsync(int id)
    {
        return _catalogRepository.GetByIdAsync(id);
    }

    public Task<Game> AddGameAsync(Game game)
    {
        return _catalogRepository.AddGameAsync(game);
    }

    public async Task<bool> UpdateGameAsync(int id, Game game)
    {
        game.Id = id;
        return await _catalogRepository.UpdateGameAsync(game);
    }

    public Task<bool> DeleteGameAsync(int id)
    {
        return _catalogRepository.DeleteGameAsync(id);
    }
}