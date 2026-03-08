using System.Text.Json;
using BigFitness.Data;
using BigFitness.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitness.Services;

public class SeedService(AppDbContext db)
{
    public async Task SeedProductsAsync()
    {
        if (await db.Products.AnyAsync().ConfigureAwait(false))
            return; // уже засеяно

        using var stream = await FileSystem.OpenAppPackageFileAsync("products_seed.json").ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync().ConfigureAwait(false);

        var items = JsonSerializer.Deserialize<List<SeedProduct>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (items is null || items.Count == 0)
            return;

        var products = items.Select(s => new Product(
            s.Name,
            s.Calories,
            s.Proteins,
            s.Fats,
            s.Carbs,
            s.DefaultPortionGrams)).ToList();

        await db.Products.AddRangeAsync(products).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private sealed class SeedProduct
    {
        public string Name                { get; set; } = "";
        public double Calories            { get; set; }
        public double Proteins            { get; set; }
        public double Fats                { get; set; }
        public double Carbs               { get; set; }
        public double DefaultPortionGrams { get; set; }
    }
}
