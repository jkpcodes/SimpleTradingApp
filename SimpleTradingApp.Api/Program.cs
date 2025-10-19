using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Inject dependencies from other layers
builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

// Apply migrations automatically on startup
// Note: Not recommended for production scenarios, a better approach is to trigger migrations in CI/CD pipeline
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

app.Run();
