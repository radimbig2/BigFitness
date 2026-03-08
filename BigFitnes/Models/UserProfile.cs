using BigFitness.Models.Enums;

namespace BigFitness.Models;

public class UserProfile
{
    // Required by EF Core
    private UserProfile() { }

    public UserProfile(
        string name,
        double height,
        double? goalWeight,
        int age,
        Gender gender,
        ActivityLevel activityLevel,
        CalorieGoalMode calorieGoalMode,
        double dailyCalorieGoal,
        double dailyProteinGoal,
        double dailyFatGoal,
        double dailyCarbGoal)
    {
        Name = name;
        Height = height;
        GoalWeight = goalWeight;
        Age = age;
        Gender = gender;
        ActivityLevel = activityLevel;
        CalorieGoalMode = calorieGoalMode;
        DailyCalorieGoal = dailyCalorieGoal;
        DailyProteinGoal = dailyProteinGoal;
        DailyFatGoal = dailyFatGoal;
        DailyCarbGoal = dailyCarbGoal;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public double Height { get; private set; }
    public double? GoalWeight { get; private set; }
    public double DailyCalorieGoal { get; private set; } = 2000;
    public double DailyProteinGoal { get; private set; } = 150;
    public double DailyFatGoal { get; private set; } = 70;
    public double DailyCarbGoal { get; private set; } = 250;
    public bool IsDarkTheme { get; private set; }
    public int Age { get; private set; }
    public Gender Gender { get; private set; }
    public ActivityLevel ActivityLevel { get; private set; }
    public CalorieGoalMode CalorieGoalMode { get; private set; }

    public void UpdatePersonalInfo(
        string name,
        double height,
        double? goalWeight,
        int age,
        Gender gender,
        ActivityLevel activityLevel)
    {
        Name = name;
        Height = height;
        GoalWeight = goalWeight;
        Age = age;
        Gender = gender;
        ActivityLevel = activityLevel;
    }

    public void SetCalorieGoalMode(CalorieGoalMode mode, double dailyCalorieGoal)
    {
        CalorieGoalMode = mode;
        DailyCalorieGoal = dailyCalorieGoal;
    }

    public void UpdateNutritionGoals(double dailyProteinGoal, double dailyFatGoal, double dailyCarbGoal)
    {
        DailyProteinGoal = dailyProteinGoal;
        DailyFatGoal = dailyFatGoal;
        DailyCarbGoal = dailyCarbGoal;
    }

    public void SetTheme(bool isDark) => IsDarkTheme = isDark;
}
