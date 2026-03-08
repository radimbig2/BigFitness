namespace BigFitness.Models;

public class Product
{
    // Required by EF Core
    private Product() { }

    public Product(
        string name,
        double calories,
        double proteins,
        double fats,
        double carbs,
        double defaultPortionGrams = 100,
        bool isCustom = false,
        bool isFavorite = false)
    {
        Name = name;
        Calories = calories;
        Proteins = proteins;
        Fats = fats;
        Carbs = carbs;
        DefaultPortionGrams = defaultPortionGrams;
        IsCustom = isCustom;
        IsFavorite = isFavorite;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public double Calories { get; private set; }
    public double Proteins { get; private set; }
    public double Fats { get; private set; }
    public double Carbs { get; private set; }
    public double DefaultPortionGrams { get; private set; } = 100;
    public bool IsCustom { get; private set; }
    public bool IsFavorite { get; private set; }

    public void Update(
        string name,
        double calories,
        double proteins,
        double fats,
        double carbs,
        double defaultPortionGrams)
    {
        Name = name;
        Calories = calories;
        Proteins = proteins;
        Fats = fats;
        Carbs = carbs;
        DefaultPortionGrams = defaultPortionGrams;
    }

    public void ToggleFavorite() => IsFavorite = !IsFavorite;
}
