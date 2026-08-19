using AstroMonitor.Domain.Entities;

namespace AstroMonitor.Application.Common.Interfaces;

public interface IStarCsvParser
{
    IEnumerable<Star> Parse(Stream csvStream, double maxMagnitude = 6.5);
}