namespace AstroMonitor.Infrastructure.Calculations;

public static class AstronomyMath
{   
    /// <summary>
    /// Обчислює Юліанську дату (JD) для заданого часу UTC.
    /// </summary>
    public static double GetJulianDate(DateTime utc)
    {
        int y = utc.Year;
        int m = utc.Month;
        double d = utc.Day + (utc.Hour / 24.0) + (utc.Minute / 1440.0) + (utc.Second / 86400.0);

        if (m <= 2)
        {
            y -= 1;
            m += 12;
        }

        int a = y / 100;
        int b = 2 - a + (a / 4);

        return Math.Truncate(365.25 * (y + 4716)) + Math.Truncate(30.6001 * (m + 1)) + d + b - 1524.5;
    }

    /// <summary>
    /// Обчислює Місцевий зоряний час (LST) у годинах.
    /// </summary>
    public static double GetLocalSiderealTime(double jd, double longitude)
    {
        double daysSinceJ2000 = jd - 2451545.0;
        
        double gmst = 18.697374558 + 24.06570982441908 * daysSinceJ2000;
        gmst %= 24.0;
        if (gmst < 0)
        {
            gmst += 24.0;
        }

        double lst = gmst + (longitude / 15.0);
        lst %= 24.0;
        if (lst < 0)
        {
            lst += 24.0;
        }

        return lst;
    }
}