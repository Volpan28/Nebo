using AstroMonitor.Domain.Enums;

namespace AstroMonitor.Domain.Entities;

public class SolarSystemBody
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public AstroBodyType BodyType { get; private set; }
    public double RadiusKm { get; private set; }
    public string Description { get; private set; }
    public string TextureIdentifier { get; private set; }
    
    private SolarSystemBody() {}

    public SolarSystemBody(string id, string name, AstroBodyType bodyType, double radiusKm, string description,
        string textureIdentifier)
    {
        Id = id;
        Name = name;
        BodyType = bodyType;
        RadiusKm = radiusKm;
        Description = description;
        TextureIdentifier = textureIdentifier;
    }
}