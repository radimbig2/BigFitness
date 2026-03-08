using ApexCharts;
using BigFitness.Models;
using Microsoft.AspNetCore.Components;
using Grid = ApexCharts.Grid;
using Label = ApexCharts.Label;
using Style = ApexCharts.Style;
using Toolbar = ApexCharts.Toolbar;

namespace BigFitness.Components.Shared;

public partial class WeightChart : ComponentBase
{
    [Parameter] public List<WeightRecord> Records { get; set; } = new();
    [Parameter] public double? GoalWeight { get; set; }

    private ApexChart<WeightRecord>? _chart;
    private ApexChartOptions<WeightRecord> _options = new();

    protected override async Task OnParametersSetAsync()
    {
        BuildOptions();
        if (_chart != null)
            await _chart.UpdateSeriesAsync(animate: true);
    }

    private void BuildOptions()
    {
        double minY = Records.Any() ? Records.Min(r => r.Weight) : 50;
        double maxY = Records.Any() ? Records.Max(r => r.Weight) : 100;
        if (GoalWeight.HasValue)
        {
            minY = Math.Min(minY, GoalWeight.Value);
            maxY = Math.Max(maxY, GoalWeight.Value);
        }
        double pad = Math.Max((maxY - minY) * 0.15, 2);
        minY -= pad;
        maxY += pad;

        _options = new ApexChartOptions<WeightRecord>
        {
            Chart = new Chart
            {
                Toolbar = new Toolbar { Show = false },
                Zoom = new Zoom { Enabled = true },
                Background = "transparent",
                FontFamily = "inherit",
            },
            Legend = new Legend { Show = true, Labels = new LegendLabels { Colors = "#aaaaaa" } },
            Stroke = new Stroke
            {
                Curve = Curve.Smooth,
                Width = new List<int> { 3 }
            },
            Markers = new Markers
            {
                Size = new List<int> { 5 },
                StrokeWidth = 2,
                Colors = new List<string> { "#29b6f6" }
            },
            Xaxis = new XAxis
            {
                Type = XAxisType.Category,
                Labels = new XAxisLabels
                {
                    Show = true,
                    Style = new AxisLabelStyle { Colors = "#aaaaaa", FontSize = "11px" }
                },
                AxisBorder = new AxisBorder { Color = "#444" },
                AxisTicks = new AxisTicks { Color = "#444" }
            },
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Show = true,
                    Min = (decimal)minY,
                    Max = (decimal)maxY,
                    Labels = new YAxisLabels
                    {
                        Show = true,
                        Formatter = "function(val) { return val.toFixed(1) + ' kg'; }",
                        Style = new AxisLabelStyle { Colors = "#aaaaaa", FontSize = "11px" }
                    }
                }
            },
            Tooltip = new Tooltip
            {
                Y = new TooltipY { Formatter = "function(val) { return val.toFixed(1) + ' kg'; }" }
            },
            Grid = new Grid
            {
                BorderColor = "#333",
                Row = new GridRow { Colors = new List<string> { "transparent" } }
            },
            Annotations = GoalWeight.HasValue ? new Annotations
            {
                Yaxis = new List<AnnotationsYAxis>
                {
                    new AnnotationsYAxis
                    {
                        Y = (decimal)GoalWeight.Value,
                        BorderColor = "#00c853",
                        BorderWidth = 2,
                        StrokeDashArray = 6,
                        Label = new Label
                        {
                            Text = $"Goal {GoalWeight:0.0} kg",
                            OffsetX = -8,
                            Style = new Style
                            {
                                Background = "#00c853",
                                Color = "#fff",
                                FontSize = "11px"
                            }
                        }
                    }
                }
            } : new Annotations()
        };
    }
}
