using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

public class PointsContext : INotifyPropertyChanged
{
    private Point[] _arr;
    private int _count;

    // Конструктор, принимающий массив и его размер
    public PointsContext(Point[] sourceArray, int size)
    {
        if (size > sourceArray.Length)
            throw new ArgumentException(nameof(size));

        _arr = new Point[size];
        Array.Copy(sourceArray, _arr, size);
        _count = size;
    }

    // Конструктор, принимающий только размер массива и заполняющий его случайными значениями
    public PointsContext(int size)
    {
        _arr = new Point[size];
        _count = size;
        
        // Параметры синусоиды
        double amplitude = 100;   // Амплитуда
        double periods = 4;       // Количество полных периодов на всем графике
        double step = 2 * Math.PI * periods / size; // Шаг по X
        
        for (int i = 0; i < size; i++)
        {
            double x = i;
            double y = amplitude * Math.Sin(i * step);
            _arr[i] = new Point(x, y);
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

    public int Count
    {
        get => _count;
        private set
        {
            _count = value;
            OnPropertyChanged();
        }
    }

    public void Add(Point point)
    {
        if (_count < _arr.Length)
        {
            _arr[_count] = point;
            _count += 1;
            Count = _count;
            OnPropertyChanged(nameof(Arr));
        }
    }

    public void UpdatePoint(int index, Point newPoint)
    {
        if (index < 0 || index >= _count)
            throw new IndexOutOfRangeException();

        _arr[index] = newPoint;
        OnPropertyChanged(nameof(Arr));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}