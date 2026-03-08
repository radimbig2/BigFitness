namespace BigFitnes.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Calories { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbs { get; set; }
    public double DefaultPortionGrams { get; set; } = 100;
    public bool IsCustom { get; set; }
    public bool IsFavorite { get; set; }
}
