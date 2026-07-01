using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Infrastructure.NasaApi;
using AstroMonitor.Infrastructure.NasaApi.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AstroMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NasaApiOptions>(
            configuration.GetSection(NasaApiOptions.SectionName));

        services.AddHttpClient<INasaClient, NasaClient>((ServiceProvider, client) =>
        {
            var options = ServiceProvider.GetRequiredService<IOptions<NasaApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });
        
        return services;
    }
}