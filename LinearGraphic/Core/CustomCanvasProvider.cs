using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Model;
using System.Linq;
using Avalonia.Controls.Shapes;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Layout;

namespace Core;

public class CustomCanvasProvider : IGraphProvider
{
    private readonly Canvas _canvas = new();
    private GraphSettings? _settings;
    private readonly Dictionary<string, Polyline> _seriesCache = new();
    private StackPanel? _legendPanel;
    private Border? _tooltipBorder;
    private TextBlock? _tooltipTextBlock;
    private double _xAxisPosition;
    private double _yAxisPosition;

    public object GetGraphControl() => _canvas;

    public void Initialize(GraphSettings settings)
    {
        _settings = settings;
        _canvas.Width = settings.ChartXLevelMax - settings.ChartXLevelMin;
        _canvas.Height = settings.ChartYLevelMax - settings.ChartYLevelMin;
        
        // Calculate axis positions
        _yAxisPosition = (0 - settings.ChartXLevelMin) * _canvas.Width / (settings.ChartXLevelMax - settings.ChartXLevelMin);
        _xAxisPosition = _canvas.Height - (settings.ChartYLevelMin - settings.ChartYLevelMin) * _canvas.Height / (settings.ChartYLevelMax - settings.ChartYLevelMin);
        
        _canvas.Children.Clear();
        DrawGrid();
        DrawAxes();
        DrawAxisLabels();
        SetupTooltip();
        SetupLegend();

        _canvas.PointerMoved += Canvas_PointerMoved;
        _canvas.PointerExited += Canvas_PointerExited;
    }

    private void SetupTooltip()
    {
        _tooltipTextBlock = new TextBlock
        {
            Background = Brushes.White,
            Foreground = Brushes.Black,
            Padding = new Thickness(4),
            FontSize = 12
        };

        _tooltipBorder = new Border
        {
            Child = _tooltipTextBlock,
            Background = Brushes.White,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(2),
            IsVisible = false
        };

        _tooltipBorder.ZIndex = 100;
        _canvas.Children.Add(_tooltipBorder);
    }

    private void SetupLegend()
    {
        _legendPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = Brushes.WhiteSmoke,
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top
        };

        Canvas.SetLeft(_legendPanel, _canvas.Width - 150);
        Canvas.SetTop(_legendPanel, 10);
        _legendPanel.ZIndex = 50;
        _canvas.Children.Add(_legendPanel);
    }

    private void UpdateLegend(Dictionary<string, Model.Point[]> series)
    {
        if (_legendPanel == null) return;
        
        _legendPanel.Children.Clear();
        
        foreach (var kv in series)
        {
            var color = GetColorForSeries(kv.Key);
            var legendItem = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(2)
            };
            
            legendItem.Children.Add(new Rectangle
            {
                Width = 15,
                Height = 10,
                Fill = new SolidColorBrush(color),
                Margin = new Thickness(0, 0, 5, 0)
            });
            
            legendItem.Children.Add(new TextBlock
            {
                Text = kv.Key,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            });
            
            _legendPanel.Children.Add(legendItem);
        }
    }

    private void DrawAxisLabels()
    {
        if (_settings == null) return;

        // X-axis labels (bottom)
        for (double x = _settings.ChartXLevelMin; x <= _settings.ChartXLevelMax; x += _settings.GridStepX)
        {
            var scaledX = (x - _settings.ChartXLevelMin) * _canvas.Width / (_settings.ChartXLevelMax - _settings.ChartXLevelMin);
            var textBlock = new TextBlock
            {
                Text = x.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            Canvas.SetLeft(textBlock, scaledX - 10);
            Canvas.SetTop(textBlock, _xAxisPosition + 5);
            _canvas.Children.Add(textBlock);
        }

        // Y-axis labels (left)
        for (double y = _settings.ChartYLevelMin; y <= _settings.ChartYLevelMax; y += _settings.GridStepY)
        {
            var scaledY = _canvas.Height - (y - _settings.ChartYLevelMin) * _canvas.Height / (_settings.ChartYLevelMax - _settings.ChartYLevelMin);
            var textBlock = new TextBlock
            {
                Text = y.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            
            Canvas.SetLeft(textBlock, _yAxisPosition - 25);
            Canvas.SetTop(textBlock, scaledY - 8);
            _canvas.Children.Add(textBlock);
        }
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_settings == null || _tooltipBorder == null || _tooltipTextBlock == null) 
            return;

        var position = e.GetPosition(_canvas);
        
        // Check if pointer is within canvas bounds
        if (position.X < 0 || position.X > _canvas.Width || position.Y < 0 || position.Y > _canvas.Height)
        {
            _tooltipBorder.IsVisible = false;
            return;
        }

        var dataX = position.X / _canvas.Width * (_settings.ChartXLevelMax - _settings.ChartXLevelMin) + _settings.ChartXLevelMin;
        var dataY = _settings.ChartYLevelMax - position.Y / _canvas.Height * (_settings.ChartYLevelMax - _settings.ChartYLevelMin);

        _tooltipTextBlock.Text = $"X: {dataX:0.00}\nY: {dataY:0.00}";
        _tooltipBorder.IsVisible = true;
        
        // Position tooltip near pointer but ensure it stays within canvas bounds
        double left = position.X + 10;
        double top = position.Y + 10;
        
        if (left + _tooltipBorder.Bounds.Width > _canvas.Width)
            left = position.X - _tooltipBorder.Bounds.Width - 10;
        if (top + _tooltipBorder.Bounds.Height > _canvas.Height)
            top = position.Y - _tooltipBorder.Bounds.Height - 10;

        Canvas.SetLeft(_tooltipBorder, left);
        Canvas.SetTop(_tooltipBorder, top);
    }

    private void Canvas_PointerExited(object? sender, PointerEventArgs e)
    {
        if (_tooltipBorder != null)
            _tooltipBorder.IsVisible = false;
    }

    public void UpdateMultipleSeries(Dictionary<string, Model.Point[]> series)
    {
        var dataLines = _canvas.Children
            .OfType<Polyline>()
            .Where(p => !ReferenceEquals(p, _tooltipBorder))
            .ToList();

        foreach (var line in dataLines)
            _canvas.Children.Remove(line);

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

        UpdateLegend(series);
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

        // X axis (at y=ChartYLevelMin position)
        _canvas.Children.Add(new Line
        {
            StartPoint = new(0, _xAxisPosition),
            EndPoint = new(_canvas.Width, _xAxisPosition),
            Stroke = Brushes.Black,
            StrokeThickness = 1
        });

        // Y axis (at x=0 position)
        _canvas.Children.Add(new Line
        {
            StartPoint = new(_yAxisPosition, 0),
            EndPoint = new(_yAxisPosition, _canvas.Height),
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