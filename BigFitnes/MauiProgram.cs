using ApexCharts;
using BigFitness.Data;
using BigFitness.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BigFitness;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddApexCharts();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "BigFitness.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<CalorieService>();
        builder.Services.AddScoped<WeightService>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<ProfileService>();
        builder.Services.AddScoped<SeedService>();
        builder.Services.AddSingleton<ThemeService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // If the DB was created by EnsureCreated (no migrations table), delete and recreate
            try
            {
                db.Database.Migrate();
            }
            catch
            {
                db.Database.EnsureDeleted();
                db.Database.Migrate();
            }

            var seeder = scope.ServiceProvider.GetRequiredService<SeedService>();
            Task.Run(() => seeder.SeedProductsAsync()).GetAwaiter().GetResult();
        }

        return app;
    }
}
