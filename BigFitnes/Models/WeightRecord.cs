namespace BigFitnes.Models;

public class WeightRecord
{
    public int Id { get; set; }
    public double Weight { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
