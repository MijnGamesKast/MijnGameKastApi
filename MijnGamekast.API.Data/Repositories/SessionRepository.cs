using Microsoft.EntityFrameworkCore;
using MijnGameKast.API.Data.Interfaces;
using MijnGameKast.API.Data.Models;

namespace MijnGameKast.API.Data.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _dbContext;
    
    public SessionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session> AddAsync(Session session)
    {
        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync();

        return session;
    }

    public async Task<Session?> GetByTokenAsync(string token)
    {
        return await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
    }

    public async Task<List<Session>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Sessions.Where(s => s.UserId == userId).ToListAsync();
    }

    public async Task<bool> RemoveByTokenAsync(string token)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null)
        {
            return false;
        }
        
        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveByUserIdAsync(int userId)
    {
        var sessions = await _dbContext.Sessions.Where(s => s.UserId == userId).ToListAsync();
        if (sessions.Count == 0)
        {
            return false;
        }
        
        _dbContext.Sessions.RemoveRange(sessions);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(Session session)
    {
        var existingSession = await _dbContext.Sessions.FindAsync(session.Id);
        if (existingSession == null)
        {
            return false;
        }
        
        existingSession.Token = session.Token;
        existingSession.UserId = session.UserId;
        existingSession.CreatedAt = session.CreatedAt;
        existingSession.ExpiresAt = session.ExpiresAt;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}