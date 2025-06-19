using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;
using Model;
using System;

namespace ViewModels;

public class LinearGraphViewModel
{
    private readonly PointsContext _pointsContext;
    private readonly GraphSettings _graphSettings;
    private readonly Axis _xAxis;
    private readonly Axis _yAxis;

    public LinearGraphViewModel(PointsContext pointsContext, GraphSettings graphSettings)
    {
        _pointsContext = pointsContext;
        _graphSettings = graphSettings;

        // Инициализация осей
        _xAxis = new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            Labeler = value => value.ToString("N2")
        };
        
        _yAxis = new Axis
        {
            CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
            CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
            CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
            CrosshairSnapEnabled = true
        };

        // Инициализация серий
        Series = new ISeries[]
        {
            new LineSeries<Point>
            {
                Values = Array.Empty<Point>(),
                Mapping = (point, index) => new LiveChartsCore.Kernel.Coordinate(point.X, point.Y),
                GeometrySize = 0
            }
        };

        // Подписка на изменения
        _pointsContext.PropertyChanged += (s, e) => UpdateSeries();
        _graphSettings.PropertyChanged += (s, e) => UpdateAxes();

        // Первоначальное обновление
        UpdateAll();
    }

    public ISeries[] Series { get; set; }
    public ICartesianAxis[] XAxes => new[] { _xAxis };
    public ICartesianAxis[] YAxes => new[] { _yAxis };

    private void UpdateAll()
    {
        UpdateSeries();
        UpdateAxes();
    }

    private void UpdateSeries()
    {
        var pointsToDisplay = _pointsContext.Arr
            .Take(_graphSettings.DisplayPoints)
            .ToArray();

        if (Series[0] is LineSeries<Point> lineSeries)
        {
            lineSeries.Values = pointsToDisplay;
            lineSeries.LineSmoothness = _graphSettings.Extrapolation ? 1 : 0;
        }
    }

    private void UpdateAxes()
    {
        _xAxis.MinLimit = _graphSettings.ChartXLevelMin;
        _xAxis.MaxLimit = _graphSettings.ChartXLevelMax;
        _yAxis.MinLimit = _graphSettings.ChartYLevelMin;
        _yAxis.MaxLimit = _graphSettings.ChartYLevelMax;
    }
}