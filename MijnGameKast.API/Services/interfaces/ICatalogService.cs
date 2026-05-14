using MijnGameKast.API.Data.Models;
using MijnGameKast.API.Data.Models.Catalog;
using MijnGameKast.API.Services.Results;

namespace MijnGameKast.API.Services.Interfaces;

public interface ICatalogService
{
    Task<List<GameResponse>> GetAllAsync();
    Task<GameResponse?> GetByIdAsync(int id);
    Task<GameResponse?> AddGameAsync(CreateGameRequest request, int? userId);
    Task<ServiceResult> UpdateGameAsync(int id, UpdateGameRequest request);
    Task<ServiceResult> DeleteGameAsync(int id);
    Task<ServiceResult> ApproveGameAsync(int id);
    Task<ServiceResult> RejectGameAsync(int id);
}