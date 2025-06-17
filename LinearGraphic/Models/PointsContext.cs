using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

public class PointsContext : INotifyPropertyChanged
{
    private Point[] _arr;
    private int _count;

    public PointsContext(int size)
    {
        _arr = new Point[size];
        _count = 0;
    }

    public Point[] Arr
    {
        get => _arr;
        private set
        {
            _arr = value;
            OnPropertyChanged();
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
            _count += 1; // Используем свойство для обновления Count
            Count = _count;
            OnPropertyChanged(nameof(Arr)); // Уведомляем об изменении массива
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}