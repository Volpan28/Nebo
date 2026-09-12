using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Infrastructure.Calculations;

namespace AstroMonitor.Infrastructure.Services;

public class AstronomyMathService : IAstronomyMathService
{
    private const double DegreesToRadians = Math.PI / 180;
    private const double RadiansToDegrees = 180.0 / Math.PI;
    
    public double CalculateAltitude(double rightAscensionHours, double declinationDegrees, 
        double latitude, double longitude, DateTime utcNow)
    {
        double jd = AstronomyMath.GetJulianDate(utcNow);

        double lst = AstronomyMath.GetLocalSiderealTime(jd, longitude);

        double haHours = lst - rightAscensionHours;

        if (haHours < 0)
        {
            haHours += 24.0;
        }

        if (haHours > 24.0)
        {
            haHours -= 24.0;
        }

        double haRadians = haHours * 15.0 * DegreesToRadians;
        double latRadians = latitude * DegreesToRadians;
        double decRadians = declinationDegrees * DegreesToRadians;

        double sinAltitude = (Math.Sin(latRadians) * Math.Sin(decRadians))
                             + (Math.Cos(latRadians) * Math.Cos(decRadians) * Math.Cos(haRadians));

        double altitudeDegrees = Math.Asin(sinAltitude) * RadiansToDegrees;
        return altitudeDegrees;
    }
}