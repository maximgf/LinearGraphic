using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Avalonia;
using SkiaSharp;
using Model;
using System.Collections.Generic;
using Avalonia.Media;

namespace Core;

/// <summary>
/// Провайдер графиков, использующий библиотеку LiveChartsCore для отрисовки.
/// Реализует интерфейс IGraphProvider для обеспечения единого способа работы с графиками
/// независимо от используемой библиотеки визуализации.
/// </summary>
public class LiveChartsProvider : IGraphProvider
{
    /// <summary>
    /// Настройки графика, используемые для конфигурации осей и других параметров отображения.
    /// </summary>
    private GraphSettings? _settings;
    
    /// <summary>
    /// Ось X для графика.
    /// </summary>
    private readonly Axis _xAxis;
    
    /// <summary>
    /// Ось Y для графика.
    /// </summary>
    private readonly Axis _yAxis;
    
    /// <summary>
    /// Основной элемент управления - декартовый график из библиотеки LiveChartsCore.
    /// </summary>
    private readonly CartesianChart _chart;
    
    /// <summary>
    /// Кэш линейных рядов для оптимизации обновления графика.
    /// Ключ - имя ряда, значение - объект ряда.
    /// </summary>
    private readonly Dictionary<string, LineSeries<Point>> _seriesCache = new();

    /// <summary>
    /// Возвращает основной элемент управления графиком.
    /// </summary>
    /// <returns>Элемент управления CartesianChart из библиотеки LiveChartsCore.</returns>
    public object GetGraphControl() => _chart;

    /// <summary>
    /// Инициализирует новый экземпляр класса LiveChartsProvider с настройками по умолчанию.
    /// </summary>
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

    /// <summary>
    /// Инициализирует график с указанными настройками.
    /// Устанавливает пределы осей и параметры сетки на основе переданных настроек.
    /// </summary>
    /// <param name="settings">Настройки графика, определяющие пределы осей и шаг сетки.</param>
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

    /// <summary>
    /// Обновляет данные для нескольких рядов графика.
    /// Если ряд с указанным именем уже существует, обновляет его данные.
    /// Если ряд не существует, создает новый ряд с соответствующими настройками.
    /// </summary>
    /// <param name="series">Словарь, где ключ - имя ряда, а значение - массив точек для отображения.</param>
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
                    GeometrySize = -1,
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

    /// <summary>
    /// Определяет цвет для ряда графика в зависимости от его имени.
    /// </summary>
    /// <param name="name">Имя ряда графика.</param>
    /// <returns>Цвет для отображения указанного ряда.</returns>
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