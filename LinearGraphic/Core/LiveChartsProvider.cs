using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Avalonia;
using SkiaSharp;
using Model;
using System.Collections.Generic;
using Avalonia.Media;

namespace Core;

public class LiveChartsProvider : IGraphProvider
{
    private GraphSettings? _settings;
    private readonly Axis _xAxis;
    private readonly Axis _yAxis;
    private readonly CartesianChart _chart;
    private readonly Dictionary<string, LineSeries<Point>> _seriesCache = new();

    public object GetGraphControl() => _chart;

    public LiveChartsProvider()
    {
        _xAxis = new Axis
        {
            Name = "X Axis",
            NamePaint = new SolidColorPaint(SKColors.Black),
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            TextSize = 12,
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
        };
        
        _yAxis = new Axis
        {
            Name = "Y Axis",
            NamePaint = new SolidColorPaint(SKColors.Black),
            LabelsPaint = new SolidColorPaint(SKColors.Black),
            TextSize = 12,
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
        };

        _chart = new CartesianChart
        {
            XAxes = new[] { _xAxis },
            YAxes = new[] { _yAxis },
            Background = Brushes.White
        };
    }

    public void Initialize(GraphSettings settings)
    {
        _settings = settings;
        _xAxis.MinLimit = settings.ChartXLevelMin;
        _xAxis.MaxLimit = settings.ChartXLevelMax;
        _yAxis.MinLimit = settings.ChartYLevelMin;
        _yAxis.MaxLimit = settings.ChartYLevelMax;
        _xAxis.SeparatorsAtCenter = false;
        _xAxis.ForceStepToMin = true;
        _xAxis.MinStep = settings.GridStepX;
        _yAxis.ForceStepToMin = true;
        _yAxis.MinStep = settings.GridStepY;
    }

    public void UpdateMultipleSeries(Dictionary<string, Point[]> series)
    {
        var newSeries = new List<ISeries>();
        
        foreach (var kv in series)
        {
            if (!_seriesCache.TryGetValue(kv.Key, out var lineSeries))
            {
                var color = GetColorForSeries(kv.Key);
                lineSeries = new LineSeries<Point>
                {
                    Name = kv.Key,
                    Values = kv.Value,
                    Mapping = (p, _) => new(p.X, p.Y),
                    GeometrySize = 0,
                    Stroke = new SolidColorPaint(color, 2),
                    Fill = null,
                    LineSmoothness = kv.Key == "Common" && _settings?.Extrapolation == true ? 1 : 0
                };
                _seriesCache[kv.Key] = lineSeries;
            }
            else
            {
                lineSeries.Values = kv.Value;
            }
            newSeries.Add(lineSeries);
        }

        _chart.Series = newSeries;
    }

    private static SKColor GetColorForSeries(string name)
    {
        return name switch
        {
            "Max" => SKColors.Red,
            "Min" => SKColors.Blue,
            "Average" => SKColors.Green,
            _ => SKColors.DarkOrange,
        };
    }
}