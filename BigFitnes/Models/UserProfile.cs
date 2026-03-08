namespace BigFitnes.Models;

public enum Gender { Male, Female }

public enum ActivityLevel { Sedentary, Light, Moderate, High, VeryHigh }

public enum CalorieGoalMode { Manual, WeightLoss, Maintenance }

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Height { get; set; }
    public double? GoalWeight { get; set; }
    public double DailyCalorieGoal { get; set; } = 2000;
    public double DailyProteinGoal { get; set; } = 150;
    public double DailyFatGoal { get; set; } = 70;
    public double DailyCarbGoal { get; set; } = 250;
    public bool IsDarkTheme { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public CalorieGoalMode CalorieGoalMode { get; set; }
}
