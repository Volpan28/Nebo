using AstroMonitor.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AstroMonitor.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
    public DateTimeOffset LastLoginDate { get; set; }
    public List<Asteroid> Asteroids { get; set; } = new();
    public List<WatchlistItem> WatchlistItems { get; set; } = new();
    
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
}