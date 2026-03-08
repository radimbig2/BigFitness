using BigFitnes.Data;
using BigFitnes.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitnes.Services;

public class CalorieService
{
    private readonly AppDbContext _db;

    public CalorieService(AppDbContext db) => _db = db;

    public async Task<List<FoodEntry>> GetEntriesForDate(DateTime date)
        => await _db.FoodEntries
            .Include(e => e.Product)
            .Where(e => e.Date.Date == date.Date)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

    public async Task<DailySummary> GetDailySummary(DateTime date)
    {
        var entries = await GetEntriesForDate(date);
        return new DailySummary
        {
            Calories = entries.Sum(e => e.TotalCalories),
            Proteins = entries.Sum(e => e.TotalProteins),
            Fats = entries.Sum(e => e.TotalFats),
            Carbs = entries.Sum(e => e.TotalCarbs)
        };
    }

    public async Task<FoodEntry> AddEntry(int productId, double grams, DateTime? date = null)
    {
        var product = await _db.Products.FindAsync(productId)
            ?? throw new InvalidOperationException("Product not found");

        var ratio = grams / 100.0;
        var entry = new FoodEntry
        {
            ProductId = productId,
            PortionGrams = grams,
            TotalCalories = Math.Round(product.Calories * ratio, 1),
            TotalProteins = Math.Round(product.Proteins * ratio, 1),
            TotalFats = Math.Round(product.Fats * ratio, 1),
            TotalCarbs = Math.Round(product.Carbs * ratio, 1),
            Date = (date ?? DateTime.Today).Date,
            CreatedAt = DateTime.Now
        };

        _db.FoodEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task DeleteEntry(int id)
    {
        var entry = await _db.FoodEntries.FindAsync(id);
        if (entry is not null)
        {
            _db.FoodEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<DateTime, double>> GetCaloriesPerDay(DateTime from, DateTime to)
    {
        var entries = await _db.FoodEntries
            .Where(e => e.Date >= from.Date && e.Date <= to.Date)
            .GroupBy(e => e.Date)
            .Select(g => new { Date = g.Key, Calories = g.Sum(e => e.TotalCalories) })
            .ToListAsync();

        return entries.ToDictionary(e => e.Date, e => e.Calories);
    }

    public async Task<List<(DateTime Hour, double Calories)>> GetCaloriesPerHour(DateTime from, DateTime to)
    {
        var entries = await _db.FoodEntries
            .Where(e => e.CreatedAt >= from && e.CreatedAt <= to)
            .ToListAsync();

        return entries
            .GroupBy(e => new DateTime(e.CreatedAt.Year, e.CreatedAt.Month, e.CreatedAt.Day, e.CreatedAt.Hour, 0, 0))
            .Select(g => (g.Key, g.Sum(e => e.TotalCalories)))
            .OrderBy(x => x.Item1)
            .ToList();
    }
}

public class DailySummary
{
    public double Calories { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbs { get; set; }
}
