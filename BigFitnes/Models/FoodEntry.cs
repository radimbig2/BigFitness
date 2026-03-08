namespace BigFitnes.Models;

public class FoodEntry
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public double PortionGrams { get; set; }
    public double TotalCalories { get; set; }
    public double TotalProteins { get; set; }
    public double TotalFats { get; set; }
    public double TotalCarbs { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
