using System.Globalization;
using System.Net.Http.Json;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Features.Asteroids.Commands.CreateAsteroids;
using AstroMonitor.Infrastructure.NasaApi.Models;
using AstroMonitor.Infrastructure.NasaApi.Options;
using Microsoft.Extensions.Options;

namespace AstroMonitor.Infrastructure.NasaApi;

public class NasaClient : INasaClient
{
    private readonly HttpClient _httpClient;
    private readonly NasaApiOptions _options;

    public NasaClient(HttpClient httpClient, IOptions<NasaApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    
    public async Task<IEnumerable<CreateAsteroidCommand>> FetchAsteroidsAsync(
        string startDate, 
        string endDate, 
        CancellationToken cancellationToken = default)
    {
        var url = $"https://www.neowsapp.com/rest/v1/feed?start_date={startDate}&end_date={endDate}&detailed=false&api_key={_options.ApiKey}";

        var response = await _httpClient.GetFromJsonAsync<NasaNeoWsResponse>(url, cancellationToken);

        if (response?.NearEarthObjects == null || response.NearEarthObjects.Count == 0)
        {
            return Enumerable.Empty<CreateAsteroidCommand>();
        }

        var command = response.NearEarthObjects
            .SelectMany(dateGroup => dateGroup.Value)
            .Select(asteroid =>
            {
                var closeApproach = asteroid.CloseApproachData.FirstOrDefault();

                var approachDate = DateTimeOffset.TryParse(closeApproach?.CloseApproachDateRaw, out var parsedDate)
                    ? parsedDate.ToUniversalTime()
                    : default;

                var velocity = double.TryParse(
                    closeApproach?.RelativeVelocity?.KilometersPerSecond,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var parsedVelocity)
                    ? parsedVelocity
                    : 0.0;

                return new CreateAsteroidCommand
                (
                    asteroid.Id,
                    asteroid.Name,
                    asteroid.EstimatedDiameter.Meters.EstimatedDiameterMin,
                    asteroid.EstimatedDiameter.Meters.EstimatedDiameterMax,
                    approachDate,
                    velocity,
                    asteroid.IsPotentiallyHazardous
                );
            })
            .ToList();
        
        return command;
    }
}