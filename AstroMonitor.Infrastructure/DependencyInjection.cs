using System.Text;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Infrastructure.Files.Parsers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AstroMonitor.Infrastructure.NasaApi;
using AstroMonitor.Infrastructure.NasaApi.Options;
using AstroMonitor.Infrastructure.Services;
using AstroMonitor.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
        
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));
        
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? throw new InvalidOperationException($"Missing {JwtOptions.SectionName}");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        services.AddScoped<IUserManager, UserManagerService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IStarCsvParser, StarCsvParser>();
        return services;
    }
}