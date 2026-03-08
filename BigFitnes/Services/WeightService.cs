using BigFitness.Data;
using BigFitness.Models;
using Microsoft.EntityFrameworkCore;

namespace BigFitness.Services;

public class WeightService
{
    private readonly AppDbContext _db;

    public WeightService(AppDbContext db) => _db = db;

    public async Task<List<WeightRecord>> GetWeightHistory(int days = 30)
    {
        var fromDate = DateTime.Today.AddDays(-days);
        return await _db.WeightRecords
            .Where(w => w.Date >= fromDate)
            .OrderBy(w => w.Date)
            .ToListAsync();
    }

    public async Task<WeightRecord> AddWeight(double weight, DateTime date)
    {
        var existing = await _db.WeightRecords
            .FirstOrDefaultAsync(w => w.Date.Date == date.Date);

        if (existing is not null)
        {
            existing.UpdateWeight(weight);
        }
        else
        {
            existing = new WeightRecord(weight, date);
            _db.WeightRecords.Add(existing);
        }

        await _db.SaveChangesAsync();
        return existing;
    }

    public double GetOptimalWeight(double heightCm)
        => Math.Round(22.0 * (heightCm / 100.0) * (heightCm / 100.0), 1);

    public async Task<WeightRecord?> GetLatestWeight()
        => await _db.WeightRecords.OrderByDescending(w => w.Date).FirstOrDefaultAsync();
}
