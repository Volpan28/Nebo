using AstroMonitor.Application.Common.Exceptions;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Add;

public class AddWatchListItemCommandHandler : IRequestHandler<AddWatchListItemCommand, string>
{
    private readonly IAMDbContext _context;
    
    public AddWatchListItemCommandHandler(IAMDbContext context)
    {
        _context = context;
    }
    
    public async Task<string> Handle(AddWatchListItemCommand request, CancellationToken cancellationToken)
    {
        bool isAlredyAdded = await _context.WatchlistItems
            .AnyAsync(w => w.UserId == request.UserId && w.ObjectId == request.ObjectId);

        if (isAlredyAdded)
        {
            throw new ItemAlreadyExistsException();
        }

        var item = new WatchlistItem(
            request.UserId,
            request.ObjectId,
            request.Note
        );
        
        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}