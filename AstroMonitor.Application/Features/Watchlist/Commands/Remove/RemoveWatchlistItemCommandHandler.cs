using AstroMonitor.Application.Common.Exceptions;
using AstroMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Remove;

public class RemoveWatchlistItemCommandHandler : IRequestHandler<RemoveWatchlistItemCommand, string>
{
    private readonly IAMDbContext _context;
    
    public RemoveWatchlistItemCommandHandler(IAMDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(RemoveWatchlistItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.UserId == request.UserId && w.ObjectId == request.ObjectId);

        if (item == null)
        {
            throw new ItemNotFoundException();
        }
        
        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.ObjectId;
    }
}