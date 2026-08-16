namespace AstroMonitor.Domain.Entities;

public class Star
{
    public string Id { get; private set; }
    public string ProperName { get; private set; }
    public double RightAscension { get; private set; }
    public double Declination { get; private set; }
    public double Distance { get; private set; }
    public double Magnitude { get; private set; }
    public double ColorIndex { get; private set; }
    
    public string ConstellationId { get; private set; }
    public Constellation Constellation { get; private set; }
    
    private Star() {}

    public Star(string id, string properName, double rightAscension, double declination, double distance,
        double magnitude, double colorIndex, string constellationId)
    {
        Id = id;
        ProperName = properName;
        RightAscension = rightAscension;
        Declination = declination;
        Distance = distance;
        Magnitude = magnitude;
        ColorIndex = colorIndex;
        ConstellationId = constellationId;
    }
}