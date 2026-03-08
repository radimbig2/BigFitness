using System.Text.Json;
using BigFitnes.Data;
using BigFitnes.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitnes.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<List<Product>> GetAllProducts()
        => await _db.Products.OrderBy(p => p.Name).ToListAsync();

    public async Task<List<Product>> SearchProducts(string query)
        => await _db.Products
            .Where(p => p.Name.ToLower().Contains(query.ToLower()))
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<List<Product>> GetFrequentProducts(int count = 10)
    {
        var frequentIds = await _db.FoodEntries
            .GroupBy(e => e.ProductId)
            .OrderByDescending(g => g.Count())
            .Take(count)
            .Select(g => g.Key)
            .ToListAsync();

        var frequentProducts = frequentIds.Count > 0
            ? await _db.Products.Where(p => frequentIds.Contains(p.Id)).ToListAsync()
            : new List<Product>();

        frequentProducts = frequentProducts.OrderBy(p => frequentIds.IndexOf(p.Id)).ToList();

        var favoriteProducts = await _db.Products
            .Where(p => p.IsFavorite && !frequentIds.Contains(p.Id))
            .OrderBy(p => p.Name)
            .ToListAsync();

        var result = frequentProducts.Concat(favoriteProducts).Take(count).ToList();

        if (result.Count == 0)
            return await _db.Products.Take(count).ToListAsync();

        return result;
    }

    public async Task<Product> AddCustomProduct(Product product)
    {
        product.IsCustom = true;
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<List<Product>> GetAllProductsSorted()
        => await _db.Products
            .OrderByDescending(p => p.IsFavorite)
            .ThenBy(p => p.Name)
            .ToListAsync();

    public async Task ToggleFavorite(int productId)
    {
        var p = await _db.Products.FindAsync(productId);
        if (p != null) { p.IsFavorite = !p.IsFavorite; await _db.SaveChangesAsync(); }
    }

    public async Task UpdateProduct(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> NameExists(string name, int excludeId = 0)
        => await _db.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.Id != excludeId);

    public async Task DeleteProduct(int productId)
    {
        var p = await _db.Products.FindAsync(productId);
        if (p != null) { _db.Products.Remove(p); await _db.SaveChangesAsync(); }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private record ProductDto(
        string Name, double Calories, double Proteins, double Fats, double Carbs,
        double DefaultPortionGrams, bool IsFavorite);

    public async Task<string> ExportToJsonAsync()
    {
        var products = await _db.Products.ToListAsync();
        var dtos = products.Select(p => new ProductDto(
            p.Name, p.Calories, p.Proteins, p.Fats, p.Carbs, p.DefaultPortionGrams, p.IsFavorite));
        var json = JsonSerializer.Serialize(dtos, _jsonOptions);

        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = Path.Combine(dir, $"bigfitnes_products_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        await File.WriteAllTextAsync(path, json);
        return path;
    }

    public async Task<(int Added, int Skipped)> ImportFromJsonAsync(string json)
    {
        var dtos = JsonSerializer.Deserialize<List<ProductDto>>(json, _jsonOptions)
                   ?? [];

        int added = 0, skipped = 0;
        var toAdd = new List<Product>();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || await NameExists(dto.Name))
            {
                skipped++;
                continue;
            }
            toAdd.Add(new Product
            {
                Name = dto.Name.Trim(),
                Calories = dto.Calories,
                Proteins = dto.Proteins,
                Fats = dto.Fats,
                Carbs = dto.Carbs,
                DefaultPortionGrams = dto.DefaultPortionGrams > 0 ? dto.DefaultPortionGrams : 100,
                IsFavorite = dto.IsFavorite,
                IsCustom = true
            });
            added++;
        }

        if (toAdd.Count > 0)
        {
            _db.Products.AddRange(toAdd);
            await _db.SaveChangesAsync();
        }

        return (added, skipped);
    }
}
