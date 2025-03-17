using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Lab5TestTask.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// Service for managing users and their sessions.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the user with the largest number of sessions.
    /// </summary>
    /// <returns>The user with the most sessions or null if no users found.</returns>
    public async Task<User?> GetUserAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving user with the most sessions");

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Sessions)
                .OrderByDescending(u => u.Sessions.Count)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("No users found in the system");
            }
            else
            {
                _logger.LogInformation("Found user with ID: {UserId} having {SessionCount} sessions", 
                    user.Id, user.Sessions.Count);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with most sessions");
            throw;
        }
    }

    /// <summary>
    /// Gets all users that have at least one mobile session.
    /// </summary>
    /// <returns>List of users with mobile sessions.</returns>
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving users with mobile sessions");

            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Sessions)
                .Where(u => u.Sessions.Any(s => s.DeviceType == DeviceType.Mobile))
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} users with mobile sessions", users.Count);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving users with mobile sessions");
            throw;
        }
    }
}
