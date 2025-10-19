using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTradingApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
            var connectionString = connectionStringTemplate
                .Replace("$POSTGRES_HOST", configuration["POSTGRES_HOST"])
                .Replace("$POSTGRES_USER", configuration["POSTGRES_USER"])
                .Replace("$POSTGRES_PASSWORD", configuration["POSTGRES_PASSWORD"])
                .Replace("$POSTGRES_PORT", configuration["POSTGRES_PORT"]);

            options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));
        });
        return services;
    }
}
