using AstroMonitor.Domain.Enums;

namespace AstroMonitor.Domain.Entities;

public class SolarSystemBody
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public AstroBodyType BodyType { get; private set; }
    public double Epoch { get; private set; }
    public double SemiMajorAxis { get; private set; }
    public double Eccentricity { get; private set; }
    public double Inclination { get; private set; }
    public double MeanAnomaly { get; private set; }
    public double ArgumentOfPeriapsis { get; private set; }
    public double LongitudeOfAscendingNode { get; private set; }
    public double RadiusKm { get; private set; }
    public string Description { get; private set; }
    public string TextureIdentifier { get; private set; }
    public string? ImageUrl { get; private set; }
    
    private SolarSystemBody() {}

    public SolarSystemBody(string id, string name, AstroBodyType bodyType, double epoch, double semiMajorAxis, 
        double eccentricity, double inclination, double meanAnomaly, 
        double argumentOfPeriapsis, double longitudeOfAscendingNode, 
        double radiusKm, string description, string textureIdentifier, string imageUrl)
    {
        Id = id;
        Name = name;
        BodyType = bodyType;
        Epoch = epoch;
        SemiMajorAxis = semiMajorAxis;
        Eccentricity = eccentricity;
        Inclination = inclination;
        MeanAnomaly = meanAnomaly;
        ArgumentOfPeriapsis = argumentOfPeriapsis;
        LongitudeOfAscendingNode = longitudeOfAscendingNode;
        RadiusKm = radiusKm;
        Description = description;
        TextureIdentifier = textureIdentifier;
        ImageUrl = imageUrl;
    }
}