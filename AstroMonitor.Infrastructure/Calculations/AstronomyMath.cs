namespace AstroMonitor.Infrastructure.Calculations;

public static class AstronomyMath
{   
    /// <summary>
    /// Обчислює Юліанську дату (JD) для заданого часу UTC.
    /// </summary>
    public static double GetJulianDate(DateTime utcNow)
    {
        DateTime utc = utcNow.ToUniversalTime();
        
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
    
    /// <summary>
    /// Розраховує поточні екваторіальні координати (RA, Dec) для планети на основі її Кеплерівських елементів.
    /// </summary>
    public static (double RightAscensionHours, double DeclinationDegrees) GetEquatorialFromKeplerian(
        double a, double e, double i, double M, double w, double node, double epoch, 
        double earthA, double earthE, double earthI, double earthM, double earthW, double earthNode, double earthEpoch, 
        DateTime utcNow)
    {
        double jd = GetJulianDate(utcNow);

        // Отримуємо геліоцентричні координати об'єкта
        var planetHeliocentric = GetHeliocentricXYZ(a, e, i, M, w, node, epoch, jd);
        
        // Отримуємо геліоцентричні координати Землі
        var earthHeliocentric = GetHeliocentricXYZ(earthA, earthE, earthI, earthM, earthW, earthNode, earthEpoch, jd);

        // Геоцентричні екліптичні координати (віднімаємо вектор Землі)
        double gx = planetHeliocentric.x - earthHeliocentric.x;
        double gy = planetHeliocentric.y - earthHeliocentric.y;
        double gz = planetHeliocentric.z - earthHeliocentric.z;

        // Перетворення екліптичних координат в екваторіальні (нахил осі Землі ~23.439 градусів)
        double obliquityRad = 23.43928 * Math.PI / 180.0;
        double eqX = gx;
        double eqY = gy * Math.Cos(obliquityRad) - gz * Math.Sin(obliquityRad);
        double eqZ = gy * Math.Sin(obliquityRad) + gz * Math.Cos(obliquityRad);

        // Розрахунок Прямого піднесення (RA) та Схилення (Dec)
        double raRad = Math.Atan2(eqY, eqX);
        if (raRad < 0) raRad += 2 * Math.PI;
        double raHours = (raRad * 180.0 / Math.PI) / 15.0;

        double distance = Math.Sqrt(eqX * eqX + eqY * eqY + eqZ * eqZ);
        double decRad = Math.Asin(eqZ / distance);
        double decDegrees = decRad * 180.0 / Math.PI;

        return (raHours, decDegrees);
    }

    private static (double x, double y, double z) GetHeliocentricXYZ(
        double a, double e, double i, double M, double w, double node, double epoch, double jd)
    {
        // Середній рух
        double n = 0.9856076686 / Math.Pow(a, 1.5);
        double days = jd - epoch;
        double meanAnomalyDeg = (M + n * days) % 360.0;
        if (meanAnomalyDeg < 0) meanAnomalyDeg += 360.0;
        double meanAnomalyRad = meanAnomalyDeg * Math.PI / 180.0;

        // Розв'язання рівняння Кеплера (Метод Ньютона) для Ексцентричної аномалії (E)
        double E_rad = meanAnomalyRad;
        for (int iter = 0; iter < 5; iter++)
        {
            E_rad = E_rad - (E_rad - e * Math.Sin(E_rad) - meanAnomalyRad) / (1 - e * Math.Cos(E_rad));
        }

        // Істинна аномалія (v) та радіус-вектор (r)
        double v_rad = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E_rad / 2));
        double r = a * (1 - e * Math.Cos(E_rad));

        // Координати в площині орбіти
        double x_prime = r * Math.Cos(v_rad);
        double y_prime = r * Math.Sin(v_rad);

        // Перетворення в 3D геліоцентричні екліптичні координати
        double i_rad = i * Math.PI / 180.0;
        double w_rad = w * Math.PI / 180.0;
        double node_rad = node * Math.PI / 180.0;

        double x = (Math.Cos(w_rad) * Math.Cos(node_rad) - Math.Sin(w_rad) * Math.Sin(node_rad) * Math.Cos(i_rad)) * x_prime
                 + (-Math.Sin(w_rad) * Math.Cos(node_rad) - Math.Cos(w_rad) * Math.Sin(node_rad) * Math.Cos(i_rad)) * y_prime;
        
        double y = (Math.Cos(w_rad) * Math.Sin(node_rad) + Math.Sin(w_rad) * Math.Cos(node_rad) * Math.Cos(i_rad)) * x_prime
                 + (-Math.Sin(w_rad) * Math.Sin(node_rad) + Math.Cos(w_rad) * Math.Cos(node_rad) * Math.Cos(i_rad)) * y_prime;
        
        double z = (Math.Sin(w_rad) * Math.Sin(i_rad)) * x_prime
                 + (Math.Cos(w_rad) * Math.Sin(i_rad)) * y_prime;

        return (x, y, z);
    }
    
    /// <summary>
    /// Обчислює горизонтальні координати (Висота та Азимут) для заданих екваторіальних координат.
    /// </summary>
    public static (double Altitude, double Azimuth) GetHorizontalCoordinates(
        double raHours, double decDegrees, double latDegrees, double lonDegrees, DateTime utcNow)
    {
        double jd = GetJulianDate(utcNow);
        double lstHours = GetLocalSiderealTime(jd, lonDegrees);

        // Годинний кут (Hour Angle)
        double haHours = lstHours - raHours;
        if (haHours < 0) haHours += 24.0;

        double haRad = haHours * 15.0 * Math.PI / 180.0;
        double decRad = decDegrees * Math.PI / 180.0;
        double latRad = latDegrees * Math.PI / 180.0;

        // 1. Висота (Altitude)
        double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) + Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(haRad);
        double altRad = Math.Asin(sinAlt);
        double altDegrees = altRad * 180.0 / Math.PI;

        // 2. Азимут (Azimuth)
        double cosAz = (Math.Sin(decRad) - Math.Sin(altRad) * Math.Sin(latRad)) / (Math.Cos(altRad) * Math.Cos(latRad));
        // Запобігаємо помилкам округлення (NaN)
        cosAz = Math.Max(-1.0, Math.Min(1.0, cosAz)); 
        
        double azRad = Math.Acos(cosAz);
        double azDegrees = azRad * 180.0 / Math.PI;

        // Коригування квадранта для азимута
        if (Math.Sin(haRad) > 0)
        {
            azDegrees = 360.0 - azDegrees;
        }

        return (altDegrees, azDegrees);
    }
    
    /// <summary>
    /// Розраховує екваторіальні координати (RA, Dec) для Сонця, інвертуючи вектор Землі.
    /// </summary>
    public static (double RightAscensionHours, double DeclinationDegrees) GetSunEquatorial(
        double earthA, double earthE, double earthI, double earthM, double earthW, double earthNode, double earthEpoch, 
        DateTime utcNow)
    {
        // Про всяк випадок гарантуємо, що час у UTC
        DateTime utc = utcNow.ToUniversalTime();
        double jd = GetJulianDate(utc);
        
        // Отримуємо геліоцентричні координати Землі
        var earthHeliocentric = GetHeliocentricXYZ(earthA, earthE, earthI, earthM, earthW, earthNode, earthEpoch, jd);

        // Вектор від Землі до Сонця — це інвертований вектор від Сонця до Землі
        double gx = -earthHeliocentric.x;
        double gy = -earthHeliocentric.y;
        double gz = -earthHeliocentric.z;

        // Перетворення екліптичних координат в екваторіальні
        double obliquityRad = 23.43928 * Math.PI / 180.0;
        double eqX = gx;
        double eqY = gy * Math.Cos(obliquityRad) - gz * Math.Sin(obliquityRad);
        double eqZ = gy * Math.Sin(obliquityRad) + gz * Math.Cos(obliquityRad);

        double raRad = Math.Atan2(eqY, eqX);
        if (raRad < 0) raRad += 2 * Math.PI;
        double raHours = (raRad * 180.0 / Math.PI) / 15.0;

        double distance = Math.Sqrt(eqX * eqX + eqY * eqY + eqZ * eqZ);
        double decRad = Math.Asin(eqZ / distance);
        double decDegrees = decRad * 180.0 / Math.PI;

        return (raHours, decDegrees);
    }
}