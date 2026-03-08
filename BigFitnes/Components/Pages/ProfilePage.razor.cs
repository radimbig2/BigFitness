using BigFitness.Models;
using BigFitness.Models.Enums;
using BigFitness.Services;
using Microsoft.AspNetCore.Components;

namespace BigFitness.Components.Pages;

public partial class ProfilePage : ComponentBase
{
    [Inject] private ProfileService ProfileSvc { get; set; } = null!;
    [Inject] private ThemeService ThemeSvc { get; set; } = null!;
    [Inject] private WeightService WeightSvc { get; set; } = null!;

    private UserProfile? _profile;
    private bool _saved;
    private double _latestWeight;

    // Intermediate fields for form binding
    private string _editName = "";
    private double _editHeight;
    private double? _editGoalWeight;
    private int _editAge;
    private Gender _editGender;
    private ActivityLevel _editActivityLevel;
    private CalorieGoalMode _editCalorieGoalMode;
    private double _editDailyCalorieGoal;
    private double _editDailyProteinGoal;
    private double _editDailyFatGoal;
    private double _editDailyCarbGoal;

    protected override async Task OnInitializedAsync()
    {
        _profile = await ProfileSvc.GetProfile();
        var weightRecord = await WeightSvc.GetLatestWeight();
        _latestWeight = weightRecord?.Weight ?? 0;
        LoadEditFields();
    }

    private void LoadEditFields()
    {
        if (_profile is null) return;
        _editName = _profile.Name;
        _editHeight = _profile.Height;
        _editGoalWeight = _profile.GoalWeight;
        _editAge = _profile.Age;
        _editGender = _profile.Gender;
        _editActivityLevel = _profile.ActivityLevel;
        _editCalorieGoalMode = _profile.CalorieGoalMode;
        _editDailyCalorieGoal = _profile.DailyCalorieGoal;
        _editDailyProteinGoal = _profile.DailyProteinGoal;
        _editDailyFatGoal = _profile.DailyFatGoal;
        _editDailyCarbGoal = _profile.DailyCarbGoal;
    }

    private async Task ToggleTheme()
    {
        if (_profile is null) return;
        _profile.SetTheme(!_profile.IsDarkTheme);
        ThemeSvc.SetTheme(_profile.IsDarkTheme);
        await ProfileSvc.UpdateProfile(_profile);
    }

    private void ToggleManualMode()
    {
        if (_editCalorieGoalMode != CalorieGoalMode.Manual)
        {
            _editCalorieGoalMode = CalorieGoalMode.Manual;
        }
        else
        {
            _editCalorieGoalMode = CalorieGoalMode.Maintenance;
            _editDailyCalorieGoal = (int)CalcTDEE();
        }
    }

    private void SelectMode(CalorieGoalMode mode)
    {
        _editCalorieGoalMode = mode;
        _editDailyCalorieGoal = mode == CalorieGoalMode.WeightLoss
            ? (int)GetWeightLossCalories()
            : (int)CalcTDEE();
    }

    private double GetActivityCoefficient(ActivityLevel level) => level switch
    {
        ActivityLevel.Sedentary => 1.2,
        ActivityLevel.Light     => 1.375,
        ActivityLevel.Moderate  => 1.55,
        ActivityLevel.High      => 1.725,
        ActivityLevel.VeryHigh  => 1.9,
        _ => 1.2
    };

    private double CalcBMR() =>
        10 * _latestWeight + 6.25 * _editHeight - 5 * _editAge
        + (_editGender == Gender.Male ? 5 : -161);

    private double CalcTDEE() => CalcBMR() * GetActivityCoefficient(_editActivityLevel);

    private double GetWeightLossCalories() => CalcTDEE() * 0.825;

    private async Task Save()
    {
        if (_profile is null) return;

        double calorieGoal = _editCalorieGoalMode switch
        {
            CalorieGoalMode.WeightLoss  => (int)GetWeightLossCalories(),
            CalorieGoalMode.Maintenance => (int)CalcTDEE(),
            _ => _editDailyCalorieGoal
        };

        _profile.UpdatePersonalInfo(_editName, _editHeight, _editGoalWeight, _editAge, _editGender, _editActivityLevel);
        _profile.SetCalorieGoalMode(_editCalorieGoalMode, calorieGoal);
        _profile.UpdateNutritionGoals(_editDailyProteinGoal, _editDailyFatGoal, _editDailyCarbGoal);
        await ProfileSvc.UpdateProfile(_profile);

        _saved = true;
        StateHasChanged();
        await Task.Delay(1500);
        _saved = false;
        StateHasChanged();
    }
}
