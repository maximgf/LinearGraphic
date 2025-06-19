using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;
using Model;
using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core;

public class LiveChartsProvider : IGraphProvider
{
    private GraphSettings? _settings;
    private readonly Axis _xAxis;
    private readonly Axis _yAxis;
    private ISeries[] _series;
    private readonly CartesianChart _chart;

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

        _series = new ISeries[]
        {
            new LineSeries<Model.Point>
            {
                Name = "Data Series",
                Values = Array.Empty<Model.Point>(),
                Mapping = (point, index) => new LiveChartsCore.Kernel.Coordinate(point.X, point.Y),
                GeometrySize = 0,
                Stroke = new SolidColorPaint(SKColors.DarkOrange, 2),
                Fill = null
            }
        };

        _chart = new CartesianChart
        {
            Series = _series,
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
    }

    public void UpdatePoints(Model.Point[] points)
    {
        if (_series[0] is LineSeries<Model.Point> lineSeries)
        {
            lineSeries.Values = points;
            lineSeries.LineSmoothness = _settings?.Extrapolation == true ? 1 : 0;
        }
    }
}
