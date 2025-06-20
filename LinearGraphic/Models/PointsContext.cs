using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

public class PointsContext : INotifyPropertyChanged
{
    protected Point[] _arr;
    protected int _count;

    public PointsContext(int size)
    {
        _arr = new Point[size];
        _count = size;
        
        var random = new Random();
        int amplitude = 100;
        
        for (int i = 0; i < size; i++)
        {
            double y = random.NextDouble() * 2 * amplitude - amplitude;  
            _arr[i] = new Point(i, y);
        }
    }

    public Point[] Arr
    {
        get => _arr;
        set
        {
            _arr = value;
            _count = _arr.Length;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Count));
        }
    }

    public int Count => _count;

    public virtual void UpdateAllPoints(Point[] newPoints)
    {
        if (newPoints.Length != _count)
            throw new ArgumentException("Invalid array size");
        
        Array.Copy(newPoints, _arr, _count);
        OnPropertyChanged(nameof(Arr));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}