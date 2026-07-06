using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Identity;
using AstroMonitor.Infrastructure.NasaApi;
using AstroMonitor.Infrastructure.NasaApi.Options;
using AstroMonitor.Infrastructure.Services;
using AstroMonitor.Infrastructure.Settings;
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
        
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddHttpClient<INasaClient, NasaClient>((ServiceProvider, client) =>
        {
            var options = ServiceProvider.GetRequiredService<IOptions<NasaApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<IUserManager, UserManagerService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        return services;
    }
}