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

    public LinearGraphViewModel(PointsContext pointsContext, GraphSettings graphSettings)
    {
        _pointsContext = pointsContext;
        _graphSettings = graphSettings;

        // Подписываемся на изменения
        _pointsContext.PropertyChanged += (s, e) => UpdateSeries();
        _graphSettings.PropertyChanged += (s, e) => UpdateAxes();

        // Инициализируем серии и оси
        UpdateSeries();
        UpdateAxes();
    }

    public ISeries[] Series { get; set; } = Array.Empty<ISeries>();
    public ICartesianAxis[] XAxes { get; set; } = Array.Empty<ICartesianAxis>();
    public ICartesianAxis[] YAxes { get; set; } = Array.Empty<ICartesianAxis>();

    private void UpdateSeries()
    {
        // Берем только необходимое количество точек согласно настройкам
        var pointsToDisplay = _pointsContext.Arr
            .Take(_graphSettings.DisplayPoints)
            .ToArray();

        Series = new ISeries[]
        {
            new LineSeries<Point>
            {
                Values = pointsToDisplay,
                Mapping = (point, index) => new LiveChartsCore.Kernel.Coordinate(point.X, point.Y),
                GeometrySize = 0,
                LineSmoothness = _graphSettings.Extrapolation ? 1 : 0
            }
        };
    }

    private void UpdateAxes()
    {
        XAxes = new ICartesianAxis[]
        {
            new Axis
            {
                CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
                CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                Labeler = value => value.ToString("N2"),
                MinLimit = _graphSettings.ChartXLevelMin,
                MaxLimit = _graphSettings.ChartXLevelMax
            }
        };

        YAxes = new ICartesianAxis[]
        {
            new Axis
            {
                CrosshairLabelsBackground = SKColors.DarkOrange.AsLvcColor(),
                CrosshairLabelsPaint = new SolidColorPaint(SKColors.DarkRed),
                CrosshairPaint = new SolidColorPaint(SKColors.DarkOrange, 1),
                CrosshairSnapEnabled = true,
                MinLimit = _graphSettings.ChartYLevelMin,
                MaxLimit = _graphSettings.ChartYLevelMax
            }
        };
    }
}