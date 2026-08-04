using AstroMonitor.Domain.Identity;

namespace AstroMonitor.Domain.Entities;

public class WatchlistItem
{
    public string Id { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public ApplicationUser User { get; init; } = default!;
    public string ObjectId { get; init; } = default!;
    public string? Note { get; set; }
    public DateTimeOffset AddedAt { get; set; }
    
    private WatchlistItem() {}

    public WatchlistItem(string userId, string objectId, string? note = null)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId;
        ObjectId = objectId;
        Note = note;
        AddedAt = DateTimeOffset.UtcNow;
    }33
}