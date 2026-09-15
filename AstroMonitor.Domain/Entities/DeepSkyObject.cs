using AstroMonitor.Domain.Enums;

namespace AstroMonitor.Domain.Entities;

public class DeepSkyObject
{
    public string Id { get; private set; }
    public string Name { get; private set; } 
    public string CatalogName { get; private set; }
    public DeepSkyObjectType Type { get; private set; }
    public double RightAscension { get; private set; } 
    public double Declination { get; private set; } 
    public double Magnitude { get; private set; } 
    public string Description { get; private set; } 
    public string ImageUrl { get; private set; } 
    
    public string ConstellationId { get; private set; }
    public Constellation Constellation { get; private set; }
    
    private DeepSkyObject() { }

    public DeepSkyObject(string id, string name, string catalogName, DeepSkyObjectType type, 
        double rightAscension, double declination, double magnitude, 
        string constellationId, string description, string imageUrl)
    {
        Id = id;
        Name = name;
        CatalogName = catalogName;
        Type = type;
        RightAscension = rightAscension;
        Declination = declination;
        Magnitude = magnitude;
        ConstellationId = constellationId;
        Description = description;
        ImageUrl = imageUrl;
    }
}