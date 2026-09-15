namespace AstroMonitor.Domain.Entities;

public class Constellation
{
    public string Id { get; private set; }
    public string LatinName { get; private set; }
    public string EnglishName { get; private set; }
    public string Description { get; private set; }
    public string Family { get; private set; }
    public string? ImageUrl { get; private set; }
    
    private Constellation() {}

    public Constellation(string id, string latinName, string englishName, string description, string family, string imageUrl)
    {
        Id = id;
        LatinName = latinName;
        EnglishName = englishName;
        Description = description;
        Family = family;
        ImageUrl = imageUrl;
    }
}