using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstroMonitor.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<AMDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IAMDbContext>(provider => 
            provider.GetRequiredService<AMDbContext>());
        
        return services;
    }
}