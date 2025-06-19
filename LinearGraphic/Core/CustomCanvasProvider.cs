using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Model;
using System.Linq;
using Avalonia.Controls.Shapes;
using System.Collections.Generic;

namespace Core;

public class CustomCanvasProvider : IGraphProvider
{
    private readonly Canvas _canvas = new Canvas();
    private GraphSettings? _settings;

    public object GetGraphControl() => _canvas;

    public void Initialize(GraphSettings settings)
    {
        _settings = settings;
        _canvas.Width = settings.ChartXLevelMax - settings.ChartXLevelMin;
        _canvas.Height = settings.ChartYLevelMax - settings.ChartYLevelMin;
        DrawAxes();
    }

    public void UpdatePoints(Model.Point[] points)
    {
        // Удаляем все существующие полилинии
        var existingPolylines = _canvas.Children.OfType<Polyline>().ToList();
        foreach (var existingPolyline in existingPolylines)
        {
            _canvas.Children.Remove(existingPolyline);
        }
        
        // Создаем новую полилинию
        var newPolyline = new Polyline
        {
            Points = new Points(points.Select(p => ScalePoint(p))),
            Stroke = Brushes.DarkOrange,
            StrokeThickness = 2
        };
        
        _canvas.Children.Add(newPolyline);
    }

    private Avalonia.Point ScalePoint(Model.Point p)
    {
        if (_settings == null) return new Avalonia.Point(0, 0);
        
        double x = (p.X - _settings.ChartXLevelMin) * _canvas.Width / (_settings.ChartXLevelMax - _settings.ChartXLevelMin);
        double y = _canvas.Height - (p.Y - _settings.ChartYLevelMin) * _canvas.Height / (_settings.ChartYLevelMax - _settings.ChartYLevelMin);
        return new Avalonia.Point(x, y);
    }

    private void DrawAxes()
    {
        if (_settings == null) return;

        // Ось X
        _canvas.Children.Add(new Line
        {
            StartPoint = new Avalonia.Point(0, _canvas.Height / 2),
            EndPoint = new Avalonia.Point(_canvas.Width, _canvas.Height / 2),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        });

        // Ось Y
        _canvas.Children.Add(new Line
        {
            StartPoint = new Avalonia.Point(_canvas.Width / 2, 0),
            EndPoint = new Avalonia.Point(_canvas.Width / 2, _canvas.Height),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        });
    }
}