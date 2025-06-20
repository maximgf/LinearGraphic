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
    private readonly Canvas _canvas = new();
    private GraphSettings? _settings;
    private readonly Dictionary<string, Polyline> _seriesCache = new();

    public object GetGraphControl() => _canvas;

    public void Initialize(GraphSettings settings)
    {
        _settings = settings;
        _canvas.Width = settings.ChartXLevelMax - settings.ChartXLevelMin;
        _canvas.Height = settings.ChartYLevelMax - settings.ChartYLevelMin;
        
        _canvas.Children.Clear();
        DrawGrid();
        DrawAxes();
    }

    public void UpdateMultipleSeries(Dictionary<string, Model.Point[]> series)
    {
        var dataLines = _canvas.Children.OfType<Polyline>().ToList();
        foreach (var line in dataLines)
        {
            _canvas.Children.Remove(line);
        }

        foreach (var kv in series)
        {
            var color = GetColorForSeries(kv.Key);
            var polyline = new Polyline
            {
                Points = new Points(kv.Value.Select(ScalePoint)),
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2
            };
            _canvas.Children.Add(polyline);
        }
    }

    private Avalonia.Point ScalePoint(Model.Point p)
    {
        if (_settings == null) return new(0, 0);
        
        double x = (p.X - _settings.ChartXLevelMin) * _canvas.Width / (_settings.ChartXLevelMax - _settings.ChartXLevelMin);
        double y = _canvas.Height - (p.Y - _settings.ChartYLevelMin) * _canvas.Height / (_settings.ChartYLevelMax - _settings.ChartYLevelMin);
        return new(x, y);
    }

    private void DrawAxes()
    {
        if (_settings == null) return;

        _canvas.Children.Add(new Line
        {
            StartPoint = new(0, _canvas.Height / 2),
            EndPoint = new(_canvas.Width, _canvas.Height / 2),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        });

        _canvas.Children.Add(new Line
        {
            StartPoint = new(_canvas.Width / 2, 0),
            EndPoint = new(_canvas.Width / 2, _canvas.Height),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        });
    }

    private void DrawGrid()
    {
        if (_settings == null) return;

        for (double x = _settings.ChartXLevelMin; x <= _settings.ChartXLevelMax; x += _settings.GridStepX)
        {
            var scaledX = (x - _settings.ChartXLevelMin) * _canvas.Width / (_settings.ChartXLevelMax - _settings.ChartXLevelMin);
            
            _canvas.Children.Add(new Line
            {
                StartPoint = new(scaledX, 0),
                EndPoint = new(scaledX, _canvas.Height),
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            });
        }

        for (double y = _settings.ChartYLevelMin; y <= _settings.ChartYLevelMax; y += _settings.GridStepY)
        {
            var scaledY = _canvas.Height - (y - _settings.ChartYLevelMin) * _canvas.Height / (_settings.ChartYLevelMax - _settings.ChartYLevelMin);
            
            _canvas.Children.Add(new Line
            {
                StartPoint = new(0, scaledY),
                EndPoint = new(_canvas.Width, scaledY),
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            });
        }
    }

    private static Color GetColorForSeries(string name)
    {
        return name switch
        {
            "Max" => Colors.Red,
            "Min" => Colors.Blue,
            "Average" => Colors.Green,
            _ => Colors.DarkOrange,
        };
    }
}