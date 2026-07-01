namespace AstroMonitor.Infrastructure.NasaApi.Options;

public class NasaApiOptions
{
    public const string SectionName = "NasaApi";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}