using BigFitness.Models;
using BigFitness.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BigFitness.Components.Pages;

public partial class CaloriesPage : ComponentBase
{
    [Inject] private CalorieService CalorieSvc { get; set; } = null!;
    [Inject] private ProfileService ProfileSvc { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private List<FoodEntry> _entries = new();
    private DailySummary _summary = new(0, 0, 0, 0);
    private UserProfile _profile = null!;
    private bool _showPicker;
    private DateTime _selectedDate = DateTime.Today;

    private string DateLabel => _selectedDate == DateTime.Today ? "Today"
        : _selectedDate == DateTime.Today.AddDays(-1) ? "Yesterday"
        : _selectedDate.ToString("d MMM yyyy");

    protected override async Task OnInitializedAsync()
    {
        _profile = await ProfileSvc.GetProfile();
        await LoadData();
    }

    private async Task LoadData()
    {
        _entries = await CalorieSvc.GetEntriesForDate(_selectedDate);
        _summary = await CalorieSvc.GetDailySummary(_selectedDate);
    }

    private async Task PrevDay()
    {
        _selectedDate = _selectedDate.AddDays(-1);
        await LoadData();
    }

    private async Task NextDay()
    {
        if (_selectedDate < DateTime.Today)
        {
            _selectedDate = _selectedDate.AddDays(1);
            await LoadData();
        }
    }

    private async Task OpenDatePicker()
    {
        await JS.InvokeVoidAsync("eval", "var el=document.getElementById('date-picker-input');if(el.showPicker)el.showPicker();else el.click();");
    }

    private async Task OnDatePicked(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var picked) && picked <= DateTime.Today)
        {
            _selectedDate = picked;
            await LoadData();
        }
    }

    private async Task AddFood((int ProductId, double Grams) item)
    {
        await CalorieSvc.AddEntry(item.ProductId, item.Grams, _selectedDate);
        await LoadData();
    }

    private async Task DeleteEntry(int id)
    {
        await CalorieSvc.DeleteEntry(id);
        await LoadData();
    }

    private double CalPct => _profile.DailyCalorieGoal > 0 ? _summary.Calories / _profile.DailyCalorieGoal : 0;

    private static string GetPercent(double value, double goal)
        => (goal > 0 ? Math.Min(value / goal * 100, 100) : 0).ToString("0", System.Globalization.CultureInfo.InvariantCulture);
}
