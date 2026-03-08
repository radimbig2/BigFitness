using BigFitness.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BigFitness.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private ThemeService ThemeSvc { get; set; } = null!;
    [Inject] private ProfileService ProfileSvc { get; set; } = null!;

    private string _currentUrl = "/";
    private string _themeCss => ThemeSvc.IsDarkTheme ? "dark-theme" : "";

    protected override async Task OnInitializedAsync()
    {
        Nav.LocationChanged += OnLocationChanged;
        _currentUrl = new Uri(Nav.Uri).AbsolutePath;

        var profile = await ProfileSvc.GetProfile();
        ThemeSvc.SetTheme(profile.IsDarkTheme);
        ThemeSvc.OnChange += OnThemeChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _currentUrl = new Uri(e.Location).AbsolutePath;
        StateHasChanged();
    }

    private void OnThemeChanged() => InvokeAsync(StateHasChanged);

    private string IsActive(string path)
        => _currentUrl.TrimEnd('/') == path.TrimEnd('/') || (_currentUrl == "/" && path == "/") ? "active" : "";

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
        ThemeSvc.OnChange -= OnThemeChanged;
    }
}
