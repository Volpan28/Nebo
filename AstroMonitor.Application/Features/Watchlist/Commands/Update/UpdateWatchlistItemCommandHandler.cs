using AstroMonitor.Application.Common.Exceptions;
using AstroMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Update;

public class UpdateWatchlistItemCommandHandler : IRequestHandler<UpdateWatchlistItemCommand, string>
{
    private readonly IAMDbContext _context;

    public UpdateWatchlistItemCommandHandler(IAMDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateWatchlistItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.UserId == request.UserId && w.ObjectId == request.ObjectId);

        if (item == null)
        {
            throw new ItemNotFoundException();
        }
        
        item.Note = request.Note;
        await _context.SaveChangesAsync(cancellationToken);
        return item.ObjectId;
    }
}