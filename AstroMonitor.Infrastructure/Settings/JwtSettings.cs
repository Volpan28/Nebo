namespace AstroMonitor.Infrastructure.Settings;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    
    public string Key { get; set; } = string.Empty;
    public  string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessExpiryMinutes { get; set; }
    public int RefreshExpiryDays { get; set; }
}