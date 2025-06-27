using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core;

/// <summary>
/// Провайдер графиков, использующий кастомную реализацию на основе Avalonia Canvas.
/// Реализует интерфейс IGraphProvider для отрисовки графиков с использованием базовых элементов Avalonia.
/// Включает функционал для отображения сетки, осей, легенды и перекрестия с координатами.
/// </summary>
public class CustomCanvasProvider : IGraphProvider
{
    /// <summary>
    /// Основной элемент Canvas для отрисовки графика.
    /// </summary>
    private readonly Canvas _canvas = new();
    
    /// <summary>
    /// Настройки графика, включая пределы осей и шаг сетки.
    /// </summary>
    private GraphSettings? _settings;

    /// <summary>
    /// Текущие данные серий для отображения на графике.
    /// </summary>
    private Dictionary<string, Model.Point[]> _currentSeriesData = new();

    /// <summary>
    /// Панель для отображения легенды графика.
    /// </summary>
    private StackPanel? _legendPanel;
    
    /// <summary>
    /// Элементы для отображения координат и перекрестия под курсором.
    /// </summary>
    private TextBlock? _tooltip;
    private Line? _crosshairX;
    private Line? _crosshairY;

    /// <summary>
    /// Отступы внутри Canvas для осей и подписей.
    /// </summary>
    private readonly Thickness _padding = new(40, 15, 15, 30);

    /// <summary>
    /// Возвращает основной элемент управления графиком.
    /// </summary>
    public object GetGraphControl() => _canvas;

    /// <summary>
    /// Инициализирует провайдер с указанными настройками графика.
    /// Настраивает базовые параметры Canvas, легенду и элементы управления.
    /// </summary>
    /// <param name="settings">Настройки графика.</param>
    public void Initialize(GraphSettings settings)
    {
        _settings = settings;
        _canvas.Margin = new Thickness(10);
        _canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
        _canvas.VerticalAlignment = VerticalAlignment.Stretch;

        SetupLegend();
        SetupTooltipAndCrosshairs();
        _canvas.SizeChanged += (sender, args) => RebuildChart();
        _canvas.PointerMoved += OnCanvasPointerMoved;
        _canvas.PointerExited += OnCanvasPointerExited;
    }

    /// <summary>
    /// Основной метод для полной перерисовки содержимого Canvas.
    /// Вызывается при изменении размера или обновлении данных.
    /// </summary>
    private void RebuildChart()
    {
        if (_settings == null) return;

        _canvas.Children.Clear();
        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;

        if (plotAreaWidth <= 0 || plotAreaHeight <= 0) return;

        DrawGrid(plotAreaWidth, plotAreaHeight);
        DrawAxes(plotAreaWidth, plotAreaHeight);
        DrawAxisLabels(plotAreaWidth, plotAreaHeight);
        DrawSeries();

        _canvas.Children.Add(_legendPanel!);
        _canvas.Children.Add(_tooltip!);
        _canvas.Children.Add(_crosshairX!);
        _canvas.Children.Add(_crosshairY!);
        UpdateLegendPosition();
    }

    /// <summary>
    /// Обновляет данные для нескольких серий на графике.
    /// </summary>
    /// <param name="series">Словарь с данными серий.</param>
    public void UpdateMultipleSeries(Dictionary<string, Model.Point[]> series)
    {
        _currentSeriesData = series ?? new Dictionary<string, Model.Point[]>();
        UpdateLegend(_currentSeriesData);
        RebuildChart();
    }

    /// <summary>
    /// Отрисовывает серии данных на графике.
    /// </summary>
    private void DrawSeries()
    {
        if (_currentSeriesData == null) return;

        foreach (var kv in _currentSeriesData)
        {
            var color = GetColorForSeries(kv.Key);
            var polyline = new Polyline
            {
                Points = new Points(kv.Value.Select(ScalePoint)),
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                ZIndex = 10
            };
            _canvas.Children.Add(polyline);
        }
    }

    /// <summary>
    /// Масштабирует точку данных в координаты Canvas с учетом отступов.
    /// </summary>
    /// <param name="p">Точка данных.</param>
    /// <returns>Точка в координатах Canvas.</returns>
    private Avalonia.Point ScalePoint(Model.Point p)
    {
        if (_settings == null) return new(0, 0);

        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;

        double xRange = _settings.ChartXLevelMax - _settings.ChartXLevelMin;
        double yRange = _settings.ChartYLevelMax - _settings.ChartYLevelMin;

        double x = _padding.Left + (p.X - _settings.ChartXLevelMin) * plotAreaWidth / xRange;
        double y = _padding.Top + plotAreaHeight - (p.Y - _settings.ChartYLevelMin) * plotAreaHeight / yRange;

        return new(x, y);
    }
    
    /// <summary>
    /// Преобразует координаты Canvas обратно в данные.
    /// </summary>
    /// <param name="p">Точка в координатах Canvas.</param>
    /// <returns>Точка данных.</returns>
    private Model.Point InverseScalePoint(Avalonia.Point p)
    {
        if (_settings == null) return new(0, 0);

        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;
        
        if (plotAreaWidth <= 0 || plotAreaHeight <= 0) return new(0,0);
        
        double xRange = _settings.ChartXLevelMax - _settings.ChartXLevelMin;
        double yRange = _settings.ChartYLevelMax - _settings.ChartYLevelMin;

        double x = ((p.X - _padding.Left) * xRange / plotAreaWidth) + _settings.ChartXLevelMin;
        double y = ((_padding.Top + plotAreaHeight - p.Y) * yRange / plotAreaHeight) + _settings.ChartYLevelMin;

        return new Model.Point(x, y);
    }

    /// <summary>
    /// Отрисовывает оси X и Y на графике.
    /// </summary>
    private void DrawAxes(double plotAreaWidth, double plotAreaHeight)
    {
        _canvas.Children.Add(new Line
        {
            StartPoint = new(_padding.Left, _padding.Top + plotAreaHeight),
            EndPoint = new(_padding.Left + plotAreaWidth, _padding.Top + plotAreaHeight),
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            ZIndex = 1
        });

        _canvas.Children.Add(new Line
        {
            StartPoint = new(_padding.Left, _padding.Top),
            EndPoint = new(_padding.Left, _padding.Top + plotAreaHeight),
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            ZIndex = 1
        });
    }

    /// <summary>
    /// Отрисовывает сетку на графике.
    /// </summary>
    private void DrawGrid(double plotAreaWidth, double plotAreaHeight)
    {
        if (_settings == null) return;

        for (double x = _settings.ChartXLevelMin; x <= _settings.ChartXLevelMax; x += _settings.GridStepX)
        {
            var scaledX = ScalePoint(new Model.Point(x, _settings.ChartYLevelMin)).X;
            _canvas.Children.Add(new Line
            {
                StartPoint = new(scaledX, _padding.Top),
                EndPoint = new(scaledX, _padding.Top + plotAreaHeight),
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5,
                ZIndex = 0
            });
        }

        for (double y = _settings.ChartYLevelMin; y <= _settings.ChartYLevelMax; y += _settings.GridStepY)
        {
            var scaledY = ScalePoint(new Model.Point(_settings.ChartXLevelMin, y)).Y;
            _canvas.Children.Add(new Line
            {
                StartPoint = new(_padding.Left, scaledY),
                EndPoint = new(_padding.Left + plotAreaWidth, scaledY),
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5,
                ZIndex = 0
            });
        }
    }

    /// <summary>
    /// Отрисовывает подписи осей X и Y.
    /// </summary>
    private void DrawAxisLabels(double plotAreaWidth, double plotAreaHeight)
    {
        if (_settings == null) return;

        for (double x = _settings.ChartXLevelMin; x <= _settings.ChartXLevelMax; x += _settings.GridStepX)
        {
            var textBlock = new TextBlock
            {
                Text = x.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
            };
            textBlock.Measure(Size.Infinity);
            var scaledX = ScalePoint(new Model.Point(x, _settings.ChartYLevelMin)).X;
            Canvas.SetLeft(textBlock, scaledX - (textBlock.DesiredSize.Width / 2));
            Canvas.SetTop(textBlock, _padding.Top + plotAreaHeight + 5);
            _canvas.Children.Add(textBlock);
        }

        for (double y = _settings.ChartYLevelMin; y <= _settings.ChartYLevelMax; y += _settings.GridStepY)
        {
            var textBlock = new TextBlock
            {
                Text = y.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
            };
            textBlock.Measure(Size.Infinity);
            var scaledY = ScalePoint(new Model.Point(_settings.ChartXLevelMin, y)).Y;
            Canvas.SetLeft(textBlock, _padding.Left - textBlock.DesiredSize.Width - 5);
            Canvas.SetTop(textBlock, scaledY - (textBlock.DesiredSize.Height / 2));
            _canvas.Children.Add(textBlock);
        }
    }
    
    #region Setup Methods
    
    /// <summary>
    /// Настраивает панель легенды графика.
    /// </summary>
    private void SetupLegend()
    {
        _legendPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = new SolidColorBrush(Colors.WhiteSmoke, 0.8),
            Margin = new Thickness(5),
            ZIndex = 50
        };
        UpdateLegendPosition();
    }
    
    /// <summary>
    /// Настраивает элементы подсказки и перекрестия.
    /// </summary>
    private void SetupTooltipAndCrosshairs()
    {
        _tooltip = new TextBlock
        {
            IsVisible = false,
            Padding = new Thickness(5),
            Background = new SolidColorBrush(Colors.LightYellow, 0.9),
            ZIndex = 100
        };

        _crosshairX = new Line
        {
            IsVisible = false,
            Stroke = Brushes.SlateGray,
            StrokeThickness = 1,
            StrokeDashArray = new AvaloniaList<double>(4, 4),
            ZIndex = 20
        };

        _crosshairY = new Line
        {
            IsVisible = false,
            Stroke = Brushes.SlateGray,
            StrokeThickness = 1,
            StrokeDashArray = new AvaloniaList<double>(4, 4),
            ZIndex = 20
        };
    }
    
    /// <summary>
    /// Обновляет содержимое легенды на основе данных серий.
    /// </summary>
    /// <param name="series">Данные серий для отображения в легенде.</param>
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
                Margin = new Thickness(4)
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

    /// <summary>
    /// Обновляет позицию легенды на графике.
    /// </summary>
    private void UpdateLegendPosition()
    {
        if (_legendPanel == null) return;
        Canvas.SetLeft(_legendPanel, _canvas.Bounds.Width - _legendPanel.DesiredSize.Width - _padding.Right - 10);
        Canvas.SetTop(_legendPanel, _padding.Top + 10);
    }

    /// <summary>
    /// Возвращает цвет для серии по ее имени.
    /// </summary>
    /// <param name="name">Имя серии.</param>
    /// <returns>Цвет для отображения серии.</returns>
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
    
    #endregion
    
    #region Pointer Events

    /// <summary>
    /// Обрабатывает перемещение указателя мыши по графику.
    /// </summary>
    private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_settings == null || _tooltip == null || _crosshairX == null || _crosshairY == null) return;

        var pos = e.GetPosition(_canvas);
        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;

        bool isInsidePlotArea = pos.X >= _padding.Left && pos.X <= _padding.Left + plotAreaWidth &&
                                pos.Y >= _padding.Top && pos.Y <= _padding.Top + plotAreaHeight;
        
        if (isInsidePlotArea)
        {
            _tooltip.IsVisible = true;
            _crosshairX.IsVisible = true;
            _crosshairY.IsVisible = true;

            _crosshairX.StartPoint = new Avalonia.Point(_padding.Left, pos.Y);
            _crosshairX.EndPoint = new Avalonia.Point(_padding.Left + plotAreaWidth, pos.Y);
            
            _crosshairY.StartPoint = new Avalonia.Point(pos.X, _padding.Top);
            _crosshairY.EndPoint = new Avalonia.Point(pos.X, _padding.Top + plotAreaHeight);

            var dataPoint = InverseScalePoint(pos);
            _tooltip.Text = $"X: {dataPoint.X:F1}\nY: {dataPoint.Y:F1}";

            Canvas.SetLeft(_tooltip, pos.X + 15);
            Canvas.SetTop(_tooltip, pos.Y + 15);
        }
        else
        {
            OnCanvasPointerExited(sender, e);
        }
    }

    /// <summary>
    /// Обрабатывает выход указателя мыши за пределы графика.
    /// </summary>
    private void OnCanvasPointerExited(object? sender, PointerEventArgs e)
    {
        if (_tooltip != null) _tooltip.IsVisible = false;
        if (_crosshairX != null) _crosshairX.IsVisible = false;
        if (_crosshairY != null) _crosshairY.IsVisible = false;
    }

    #endregion
}