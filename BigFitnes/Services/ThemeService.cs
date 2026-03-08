namespace BigFitnes.Services;

public class ThemeService
{
    public bool IsDarkTheme { get; private set; }

    public event Action? OnChange;

    public void SetTheme(bool isDark)
    {
        if (IsDarkTheme == isDark) return;
        IsDarkTheme = isDark;
        OnChange?.Invoke();
    }

    public void Toggle() => SetTheme(!IsDarkTheme);
}
