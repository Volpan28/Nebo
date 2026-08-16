using AstroMonitor.Domain.Enums;

namespace AstroMonitor.Domain.Entities;

public class AstronomicalEvent
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public AstroEventType EventType { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public DateTimeOffset PeakDate { get; private set; }
    public bool IsVisibleNakedEye { get; private set; }
    
    private AstronomicalEvent() {}

    public AstronomicalEvent(string id, string title, string description, AstroEventType eventType,
        DateTimeOffset startDate, DateTimeOffset endDate, DateTimeOffset peakDate, bool isVisibleNakedEye)
    {
        Id = id;
        Title = title;
        Description = description;
        EventType = eventType;
        StartDate = startDate;
        EndDate = endDate;
        PeakDate = peakDate;
        IsVisibleNakedEye = isVisibleNakedEye;
    }
}