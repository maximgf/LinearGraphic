using System.Collections.Generic;
using System.Linq;

namespace Model;

/// <summary>
/// Расширенный контекст точек, хранящий историю изменений и статистические данные.
/// Наследуется от PointsContext и добавляет функционал для расчета максимальных,
/// минимальных и средних значений на основе истории данных.
/// </summary>
public class MultiPointsContext : PointsContext
{
    private readonly List<Point[]> _history = new();
    private const int HistorySize = 10;

    /// <summary>
    /// Массив точек с максимальными значениями Y для каждой координаты X.
    /// </summary>
    public Point[]? MaxPoints { get; private set; }

    /// <summary>
    /// Массив точек с минимальными значениями Y для каждой координаты X.
    /// </summary>
    public Point[]? MinPoints { get; private set; }

    /// <summary>
    /// Массив точек со средними значениями Y для каждой координаты X.
    /// </summary>
    public Point[]? AvgPoints { get; private set; }

    /// <summary>
    /// Инициализирует новый экземпляр класса MultiPointsContext с указанным размером.
    /// </summary>
    /// <param name="size">Количество точек в массиве.</param>
    public MultiPointsContext(int size) : base(size) { }

    /// <summary>
    /// Добавляет новый набор точек в историю и пересчитывает статистику.
    /// </summary>
    /// <param name="points">Массив точек для добавления в историю.</param>
    public void AddHistory(Point[] points)
    {
        _history.Add(points.Select(p => new Point(p.X, p.Y)).ToArray());
        if (_history.Count > HistorySize)
        {
            _history.RemoveAt(0);
        }
        CalculateStats();
    }

    /// <summary>
    /// Вычисляет статистические данные (максимум, минимум, среднее) на основе истории.
    /// </summary>
    private void CalculateStats()
    {
        if (_history.Count == 0) return;

        int pointCount = _history[0].Length;
        MaxPoints = new Point[pointCount];
        MinPoints = new Point[pointCount];
        AvgPoints = new Point[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var x = _history[0][i].X;
            var values = _history.Select(arr => arr[i].Y).ToArray();
            
            MaxPoints[i] = new Point(x, values.Max());
            MinPoints[i] = new Point(x, values.Min());
            AvgPoints[i] = new Point(x, values.Average());
        }
        
        OnPropertyChanged(nameof(MaxPoints));
        OnPropertyChanged(nameof(MinPoints));
        OnPropertyChanged(nameof(AvgPoints));
    }

    /// <summary>
    /// Обновляет все точки и добавляет их в историю.
    /// </summary>
    /// <param name="newPoints">Новый массив точек.</param>
    public override void UpdateAllPoints(Point[] newPoints)
    {
        base.UpdateAllPoints(newPoints);
        AddHistory(newPoints);
    }
}