using System.ComponentModel;

namespace Model;

public class PointsContext : INotifyPropertyChanged
{
    private Point[] arr { get; }

    public PointsContext(int size)
    {
        arr = new Point[size];
    }

    private int count = 0;

    public void Add(Point point)
    {
        arr[count] = point;
        count++;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}