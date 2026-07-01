using System.Text.Json.Serialization;

namespace AstroMonitor.Infrastructure.NasaApi.Models;

public class NasaNeoWsResponse
{
    [JsonPropertyName("element_count")]
    public int ElementCount { get; set; }
    
    [JsonPropertyName("near_earth_objects")]
    public Dictionary<string, List<NasaAsteroidDto>> NearEarthObjects { get; set; }
}

public class NasaAsteroidDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("is_potentially_hazardous_asteroid")]
    public bool IsPotentiallyHazardous { get; set; }

    [JsonPropertyName("estimated_diameter")]
    public NasaEstimatedDiameter EstimatedDiameter { get; set; } = new();

    [JsonPropertyName("close_approach_data")]
    public List<NasaCloseApproachData> CloseApproachData { get; set; } = new();
}

public class NasaEstimatedDiameter
{
    [JsonPropertyName("meters")] 
    public NasaDiameterBounds Meters { get; set; } = new();
}

public class NasaDiameterBounds
{
   [JsonPropertyName("estimated_diameter_min")]
   public double EstimatedDiameterMin { get; set; }
   
   [JsonPropertyName("estimated_diameter_max")]
   public double EstimatedDiameterMax { get; set; }
}

public class NasaCloseApproachData
{
    [JsonPropertyName("close_approach_date")]
    public string CloseApproachDateRaw { get; set; } = string.Empty;
    
    [JsonPropertyName("relative_velocity")]
    public NasaRelativeVelocity RelativeVelocity { get; set; } = new();
}

public class NasaRelativeVelocity
{
    [JsonPropertyName("kilometers_per_second")]
    public string KilometersPerSecond { get; set; } = string.Empty;
}