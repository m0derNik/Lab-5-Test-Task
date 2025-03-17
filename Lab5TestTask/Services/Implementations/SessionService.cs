using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Lab5TestTask.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// Service for managing user sessions.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionService> _logger;
    private static readonly DateTime FutureDateThreshold = new(2025, 1, 1);

    public SessionService(ApplicationDbContext context, ILogger<SessionService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the earliest desktop session.
    /// </summary>
    /// <returns>The earliest desktop session or null if no sessions found.</returns>
    public async Task<Session?> GetSessionAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving earliest desktop session");
            
            var session = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.DeviceType == DeviceType.Desktop)
                .OrderBy(s => s.StartedAtUTC)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                _logger.LogWarning("No desktop sessions found");
            }
            else
            {
                _logger.LogInformation("Found earliest desktop session with ID: {SessionId}", session.Id);
            }

            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving earliest desktop session");
            throw;
        }
    }

    /// <summary>
    /// Gets all sessions from active users that ended before 2025.
    /// </summary>
    /// <returns>List of sessions matching the criteria.</returns>
    public async Task<List<Session>> GetSessionsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving active user sessions ended before 2025");

            var sessions = await _context.Sessions
                .AsNoTracking()
                .Include(s => s.User)
                .Where(s => s.User.Status == UserStatus.Active && s.EndedAtUTC < FutureDateThreshold)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} sessions matching criteria", sessions.Count);
            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving sessions");
            throw;
        }
    }
}
