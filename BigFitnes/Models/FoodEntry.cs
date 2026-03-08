namespace BigFitness.Models;

public class FoodEntry
{
    // Required by EF Core
    private FoodEntry() { }

    public FoodEntry(
        int productId,
        double portionGrams,
        double totalCalories,
        double totalProteins,
        double totalFats,
        double totalCarbs,
        DateTime date)
    {
        ProductId = productId;
        PortionGrams = portionGrams;
        TotalCalories = totalCalories;
        TotalProteins = totalProteins;
        TotalFats = totalFats;
        TotalCarbs = totalCarbs;
        Date = date;
        CreatedAt = DateTime.Now;
    }

    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public double PortionGrams { get; private set; }
    public double TotalCalories { get; private set; }
    public double TotalProteins { get; private set; }
    public double TotalFats { get; private set; }
    public double TotalCarbs { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
