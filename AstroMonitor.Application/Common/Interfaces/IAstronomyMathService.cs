namespace AstroMonitor.Application.Common.Interfaces;

public interface IAstronomyMathService
{
    /// <summary>
    /// Обчислює висоту об'єкта над горизонтом у градусах.
    /// Якщо значення більше за 0 — об'єкт видимий.
    /// </summary>
    /// <param name="rightAscensionHours">Пряме піднесення об'єкта у годинах (RA з БД)</param>
    /// <param name="declinationDegrees">Схилення об'єкта у градусах (Dec з БД)</param>
    /// <param name="latitude">Широта спостерігача у градусах</param>
    /// <param name="longitude">Довгота спостерігача у градусах</param>
    /// <param name="utcNow">Поточний час UTC</param>
    /// <returns>Висота над горизонтом у градусах (-90 до +90)</returns>
    double CalculateAltitude(double rightAscensionHours, double declinationDegrees, double latitude, double longitude, DateTime utcNow);
}