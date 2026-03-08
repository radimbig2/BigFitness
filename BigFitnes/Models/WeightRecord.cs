namespace BigFitness.Models;

public class WeightRecord
{
    // Required by EF Core
    private WeightRecord() { }

    public WeightRecord(double weight, DateTime date)
    {
        Weight = weight;
        Date = date.Date;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; private set; }
    public double Weight { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void UpdateWeight(double weight)
    {
        Weight = weight;
        CreatedAt = DateTime.Now;
    }
}
