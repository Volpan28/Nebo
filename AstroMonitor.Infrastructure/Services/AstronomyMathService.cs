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
    
    public (double RightAscensionHours, double DeclinationDegrees) GetEquatorialFromKeplerian(
        double a, double e, double i, double M, double w, double node, double epoch, 
        double earthA, double earthE, double earthI, double earthM, double earthW, double earthNode, double earthEpoch, 
        DateTime utcNow)
    {
        return AstronomyMath.GetEquatorialFromKeplerian(
            a, e, i, M, w, node, epoch, 
            earthA, earthE, earthI, earthM, earthW, earthNode, earthEpoch, 
            utcNow);
    }

    public (double Altitude, double Azimuth) GetHorizontalCoordinates(double raHours, double decDegrees, double latDegrees,
        double lonDegrees, DateTime utcNow)
    {
        return AstronomyMath.GetHorizontalCoordinates(raHours, decDegrees, latDegrees, lonDegrees, utcNow);
    }
}