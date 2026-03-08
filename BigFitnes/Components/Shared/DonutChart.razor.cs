using Microsoft.AspNetCore.Components;

namespace BigFitness.Components.Shared;

public partial class DonutChart : ComponentBase
{
    [Parameter] public double Consumed { get; set; }
    [Parameter] public double Goal { get; set; } = 2000;

    private double RawPercentage => Goal > 0 ? Consumed / Goal : 0;
    private double Percentage => Math.Min(RawPercentage, 1.0);
    private double Circumference => 2 * Math.PI * 80;
    private string DashArray => $"{(Circumference * Percentage).ToString(System.Globalization.CultureInfo.InvariantCulture)} {Circumference.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
    private string DashOffset => "0";

    private string ProgressColor => RawPercentage switch
    {
        >= 1.0 => "#c62828",
        >= 0.9 => "#fbc02d",
        >= 0.7 => "#29b6f6",
        _ => "#4caf50"
    };

    private string Emoji => RawPercentage switch
    {
        >= 1.0 => "🤬",
        >= 0.9 => "🧐",
        >= 0.7 => "🙂",
        _ => "😋"
    };
}
