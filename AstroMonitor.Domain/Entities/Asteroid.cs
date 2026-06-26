using AstroMonitor.Domain.Identity;

namespace AstroMonitor.Domain.Entities;

public class Asteroid
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public double MinDiameterMeters { get; private set; }
    public double MaxDiameterMeters { get; private set; }
    public DateTimeOffset ClosestApproachDate { get; private set; }
    public double RelativeVelocityKmPerSec { get; private set; }
    public bool IsPotentiallyHazardous { get; private set; }
    
    public List<ApplicationUser> Users { get; private set; }

    public Asteroid(string id, string name, double minDiameterMeters,
        double maxDiameterMeters, DateTimeOffset closestApproachDate,
        double relativeVelocityKmPerSec, bool isPotentiallyHazardous)
    {
        Id = id;
        Name = name;
        MinDiameterMeters = minDiameterMeters;
        MaxDiameterMeters = maxDiameterMeters;
        ClosestApproachDate = closestApproachDate;
        RelativeVelocityKmPerSec = relativeVelocityKmPerSec;
        IsPotentiallyHazardous = isPotentiallyHazardous;
        Users = new List<ApplicationUser>();
    }

    private Asteroid()
    {
        Users = new List<ApplicationUser>();
    }
}