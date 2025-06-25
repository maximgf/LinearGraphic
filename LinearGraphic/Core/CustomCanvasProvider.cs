using Avalonia;
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

public class CustomCanvasProvider : IGraphProvider
{
    private readonly Canvas _canvas = new();
    private GraphSettings? _settings;
    
    // Словарь для хранения последних полученных данных для перерисовки
    private Dictionary<string, Model.Point[]> _currentSeriesData = new();

    // Элементы для всплывающей подсказки
    private Border? _tooltipBorder;
    private TextBlock? _tooltipTextBlock;
    
    // Панель для легенды
    private StackPanel? _legendPanel;

    // Отступы внутри Canvas для осей и подписей. График будет рисоваться в оставшейся области.
    private readonly Thickness _padding = new(40, 15, 15, 30); // left, top, right, bottom

    public object GetGraphControl() => _canvas;

    public void Initialize(GraphSettings settings)
    {
        _settings = settings;

        // Устанавливаем внешний отступ для всего Canvas
        _canvas.Margin = new Thickness(10);
        
        // Canvas будет занимать все доступное пространство родителя
        _canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
        _canvas.VerticalAlignment = VerticalAlignment.Stretch;

        SetupTooltip();
        SetupLegend();
        
        _canvas.PointerMoved += Canvas_PointerMoved;
        _canvas.PointerExited += Canvas_PointerExited;
        
        // Подписываемся на событие изменения размера Canvas
        _canvas.SizeChanged += (sender, args) => RebuildChart();
    }

    /// <summary>
    /// Центральный метод для полной перерисовки всего содержимого Canvas.
    /// Вызывается при изменении размера или обновлении данных.
    /// </summary>
    private void RebuildChart()
    {
        if (_settings == null) return;

        _canvas.Children.Clear();

        // Размеры области, доступной для рисования самого графика (за вычетом отступов)
        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;

        // Проверка, что область для рисования имеет корректные размеры
        if (plotAreaWidth <= 0 || plotAreaHeight <= 0) return;

        DrawGrid(plotAreaWidth, plotAreaHeight);
        DrawAxes(plotAreaWidth, plotAreaHeight);
        DrawAxisLabels(plotAreaWidth, plotAreaHeight);
        DrawSeries();

        // Передобавляем элементы, которые должны быть поверх всего
        _canvas.Children.Add(_tooltipBorder!);
        _canvas.Children.Add(_legendPanel!);
        UpdateLegendPosition();
    }
    
    public void UpdateMultipleSeries(Dictionary<string, Model.Point[]> series)
    {
        // Сохраняем новые данные и запускаем полную перерисовку
        _currentSeriesData = series ?? new Dictionary<string, Model.Point[]>();
        UpdateLegend(_currentSeriesData);
        RebuildChart();
    }
    
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
                ZIndex = 10 // Графики должны быть выше сетки и осей
            };
            _canvas.Children.Add(polyline);
        }
    }

    private Avalonia.Point ScalePoint(Model.Point p)
    {
        if (_settings == null) return new(0, 0);

        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;

        double xRange = _settings.ChartXLevelMax - _settings.ChartXLevelMin;
        double yRange = _settings.ChartYLevelMax - _settings.ChartYLevelMin;

        // Масштабируем и смещаем точку с учетом отступов (padding)
        double x = _padding.Left + (p.X - _settings.ChartXLevelMin) * plotAreaWidth / xRange;
        double y = _padding.Top + plotAreaHeight - (p.Y - _settings.ChartYLevelMin) * plotAreaHeight / yRange;
        
        return new(x, y);
    }
    
    private void DrawAxes(double plotAreaWidth, double plotAreaHeight)
    {
        // Ось X
        _canvas.Children.Add(new Line
        {
            StartPoint = new(_padding.Left, _padding.Top + plotAreaHeight),
            EndPoint = new(_padding.Left + plotAreaWidth, _padding.Top + plotAreaHeight),
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            ZIndex = 1
        });

        // Ось Y
        _canvas.Children.Add(new Line
        {
            StartPoint = new(_padding.Left, _padding.Top),
            EndPoint = new(_padding.Left, _padding.Top + plotAreaHeight),
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            ZIndex = 1
        });
    }

    private void DrawGrid(double plotAreaWidth, double plotAreaHeight)
    {
        if (_settings == null) return;
        
        // Вертикальные линии сетки
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

        // Горизонтальные линии сетки
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

    private void DrawAxisLabels(double plotAreaWidth, double plotAreaHeight)
    {
        if (_settings == null) return;

        // Подписи оси X
        for (double x = _settings.ChartXLevelMin; x <= _settings.ChartXLevelMax; x += _settings.GridStepX)
        {
            var textBlock = new TextBlock
            {
                Text = x.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
            };
            
            // Принудительно измеряем элемент, чтобы его DesiredSize было рассчитано
            textBlock.Measure(Size.Infinity);
            
            var scaledX = ScalePoint(new Model.Point(x, _settings.ChartYLevelMin)).X;
            
            // Центрируем текст под риской на оси
            Canvas.SetLeft(textBlock, scaledX - (textBlock.DesiredSize.Width / 2));
            Canvas.SetTop(textBlock, _padding.Top + plotAreaHeight + 5);
            _canvas.Children.Add(textBlock);
        }

        // Подписи оси Y
        for (double y = _settings.ChartYLevelMin; y <= _settings.ChartYLevelMax; y += _settings.GridStepY)
        {
            var textBlock = new TextBlock
            {
                Text = y.ToString("0"),
                FontSize = 10,
                Foreground = Brushes.Black,
            };
            
            // Принудительно измеряем элемент, чтобы его DesiredSize было рассчитано
            textBlock.Measure(Size.Infinity);
            
            var scaledY = ScalePoint(new Model.Point(_settings.ChartXLevelMin, y)).Y;
            
            // Позиционируем текст так, чтобы его правый край находился левее оси Y.
            // Отступ от оси составляет 5 пикселей.
            double textWidth = textBlock.DesiredSize.Width;
            Canvas.SetLeft(textBlock, _padding.Left - textWidth - 5); 
            
            // Центрируем текст по вертикали относительно риски на оси
            Canvas.SetTop(textBlock, scaledY - (textBlock.DesiredSize.Height / 2)); 
            _canvas.Children.Add(textBlock);
        }
    }
    
    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_settings == null || _tooltipBorder == null || _tooltipTextBlock == null) return;

        var position = e.GetPosition(_canvas);

        var plotAreaWidth = _canvas.Bounds.Width - _padding.Left - _padding.Right;
        var plotAreaHeight = _canvas.Bounds.Height - _padding.Top - _padding.Bottom;
        
        // Проверяем, находится ли курсор внутри области построения графика
        if (position.X < _padding.Left || position.X > _padding.Left + plotAreaWidth || 
            position.Y < _padding.Top || position.Y > _padding.Top + plotAreaHeight)
        {
            _tooltipBorder.IsVisible = false;
            return;
        }

        // Пересчитываем координаты курсора в значения данных
        var dataX = _settings.ChartXLevelMin + (position.X - _padding.Left) / plotAreaWidth * (_settings.ChartXLevelMax - _settings.ChartXLevelMin);
        var dataY = _settings.ChartYLevelMin + (_padding.Top + plotAreaHeight - position.Y) / plotAreaHeight * (_settings.ChartYLevelMax - _settings.ChartYLevelMin);

        _tooltipTextBlock.Text = $"X: {dataX:0.00}\nY: {dataY:0.00}";
        _tooltipBorder.IsVisible = true;
        
        // Позиционирование всплывающей подсказки
        double left = position.X + 15;
        double top = position.Y + 15;
        
        // Корректируем положение, если подсказка выходит за границы Canvas
        if (left + _tooltipBorder.Bounds.Width > _canvas.Bounds.Width)
            left = position.X - _tooltipBorder.Bounds.Width - 15;
        if (top + _tooltipBorder.Bounds.Height > _canvas.Bounds.Height)
            top = position.Y - _tooltipBorder.Bounds.Height - 15;

        Canvas.SetLeft(_tooltipBorder, left);
        Canvas.SetTop(_tooltipBorder, top);
    }
    
    private void Canvas_PointerExited(object? sender, PointerEventArgs e)
    {
        if (_tooltipBorder != null)
            _tooltipBorder.IsVisible = false;
    }
    
    #region Setup Methods
    private void SetupTooltip()
    {
        _tooltipTextBlock = new TextBlock
        {
            Background = Brushes.Transparent, // Фон задается в Border
            Foreground = Brushes.Black,
            Padding = new Thickness(4),
            FontSize = 12
        };

        _tooltipBorder = new Border
        {
            Child = _tooltipTextBlock,
            Background = new SolidColorBrush(Colors.White, 0.8), // Полупрозрачный фон
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(2),
            IsVisible = false,
            ZIndex = 100 // Поверх всего
        };
    }
    
    private void SetupLegend()
    {
        _legendPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Background = new SolidColorBrush(Colors.WhiteSmoke, 0.8),
            Margin = new Thickness(5),
            ZIndex = 50 // Поверх графика, но ниже подсказки
        };
        UpdateLegendPosition();
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

    private void UpdateLegendPosition()
    {
        if (_legendPanel == null) return;
        // Позиционируем легенду в правом верхнем углу области построения
        Canvas.SetLeft(_legendPanel, _canvas.Bounds.Width - _legendPanel.DesiredSize.Width - _padding.Right - 10);
        Canvas.SetTop(_legendPanel, _padding.Top + 10);
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
    #endregion
}