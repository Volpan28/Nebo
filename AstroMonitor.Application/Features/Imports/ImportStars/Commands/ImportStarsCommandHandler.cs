using AstroMonitor.Application.Common.Interfaces;
using MediatR;

namespace AstroMonitor.Application.Features.Imports.ImportStars.Commands;

public class ImportStarsCommandHandler : IRequestHandler<ImportStarsCommand>
{
    private readonly IAMDbContext _context;
    private readonly IStarCsvParser _starCsvParser;

    public ImportStarsCommandHandler(IAMDbContext context, IStarCsvParser starCsvParser)
    {
        _context = context;
        _starCsvParser = starCsvParser;
    }
    
    public async Task Handle(ImportStarsCommand request, CancellationToken cancellationToken)
    {
        var stars = _starCsvParser.Parse(request.stream);
        _context.Stars.AddRange(stars);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
