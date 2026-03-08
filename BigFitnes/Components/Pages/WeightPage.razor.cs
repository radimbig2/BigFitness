using BigFitness.Models;
using BigFitness.Models.Enums;
using BigFitness.Services;
using Microsoft.AspNetCore.Components;

namespace BigFitness.Components.Pages;

public partial class WeightPage : ComponentBase
{
    [Inject] private WeightService WeightSvc { get; set; } = null!;
    [Inject] private ProfileService ProfileSvc { get; set; } = null!;
    [Inject] private CalorieService CalorieSvc { get; set; } = null!;

    private double _weightInput;
    private WeightRecord? _latestWeight;
    private UserProfile _profile = null!;
    private List<WeightRecord> _history = new();
    private double _optimalWeight;
    private List<CaloriePoint> _caloriePoints = new();
    private double _maintenanceCalories;
    private double _weightLossCalories;

    private string _toGoalText => (_latestWeight, _profile.GoalWeight) switch
    {
        ({ } w, { } g) when Math.Abs(w.Weight - g) < 0.1 => "Reached!",
        ({ } w, { } g) => $"{Math.Abs(w.Weight - g):0.0} kg",
        _ => "—"
    };

    private string _toGoalClass => (_latestWeight, _profile.GoalWeight) switch
    {
        ({ } w, { } g) when Math.Abs(w.Weight - g) < 0.1 => "stat-card--goal-reached",
        _ => ""
    };

    protected override async Task OnInitializedAsync()
    {
        _profile = await ProfileSvc.GetProfile();
        _optimalWeight = WeightSvc.GetOptimalWeight(_profile.Height);
        await LoadData();
        _weightInput = _latestWeight?.Weight ?? 70;
    }

    private async Task LoadData()
    {
        _history = await WeightSvc.GetWeightHistory(30);
        _latestWeight = await WeightSvc.GetLatestWeight();
        await LoadCalorieChart();
    }

    private async Task LoadCalorieChart()
    {
        if (_latestWeight is not null && _profile.Age > 0)
        {
            double w = _latestWeight.Weight;
            double bmr = 10 * w + 6.25 * _profile.Height - 5 * _profile.Age
                + (_profile.Gender == Gender.Male ? 5 : -161);
            double coeff = _profile.ActivityLevel switch
            {
                ActivityLevel.Light    => 1.375,
                ActivityLevel.Moderate => 1.55,
                ActivityLevel.High     => 1.725,
                ActivityLevel.VeryHigh => 1.9,
                _ => 1.2
            };
            _maintenanceCalories = bmr * coeff;
            _weightLossCalories  = _maintenanceCalories * 0.825;
        }

        var from = DateTime.Now.AddDays(-7);
        var hourly = await CalorieSvc.GetCaloriesPerHour(from, DateTime.Now);

        _caloriePoints = hourly
            .Select(x => new CaloriePoint { Hour = x.Hour, Calories = x.Calories })
            .ToList();
    }

    private async Task SaveWeight()
    {
        if (_weightInput > 0)
        {
            await WeightSvc.AddWeight(_weightInput, DateTime.Today);
            await LoadData();
        }
    }
}
