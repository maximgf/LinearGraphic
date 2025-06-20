using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Model;

public class MultiPointsContext : PointsContext
{
    private readonly List<Point[]> _history = new();
    private const int HistorySize = 10;

    public Point[]? MaxPoints { get; private set; }
    public Point[]? MinPoints { get; private set; }
    public Point[]? AvgPoints { get; private set; }

    public MultiPointsContext(int size) : base(size) { }

    public void AddHistory(Point[] points)
    {
        _history.Add(points.Select(p => new Point(p.X, p.Y)).ToArray());
        if (_history.Count > HistorySize)
        {
            _history.RemoveAt(0);
        }
        CalculateStats();
    }

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

    public override void UpdateAllPoints(Point[] newPoints)
    {
        base.UpdateAllPoints(newPoints);
        AddHistory(newPoints);
    }
}