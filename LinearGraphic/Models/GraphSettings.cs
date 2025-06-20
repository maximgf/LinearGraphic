using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

public class GraphSettings : INotifyPropertyChanged
{
    private bool _extrapolation;
    private int _displayPoints = 1000;
    private int _chartYLevelMin = -150;
    private int _chartYLevelMax = 150;
    private int _chartXLevelMin = 0;
    private int _chartXLevelMax = 1000;
    private int _gridStepX = 100;
    private int _gridStepY = 50;

    public bool Extrapolation
    {
        get => _extrapolation;
        set { _extrapolation = value; OnPropertyChanged(); }
    }

    public int DisplayPoints
    {
        get => _displayPoints;
        set { _displayPoints = value; OnPropertyChanged(); }
    }

    public int ChartYLevelMin
    {
        get => _chartYLevelMin;
        set { _chartYLevelMin = value; OnPropertyChanged(); }
    }

    public int ChartYLevelMax
    {
        get => _chartYLevelMax;
        set { _chartYLevelMax = value; OnPropertyChanged(); }
    }

    public int ChartXLevelMin
    {
        get => _chartXLevelMin;
        set { _chartXLevelMin = value; OnPropertyChanged(); }
    }

    public int ChartXLevelMax
    {
        get => _chartXLevelMax;
        set { _chartXLevelMax = value; OnPropertyChanged(); }
    }

    public int GridStepX
    {
        get => _gridStepX;
        set { _gridStepX = value; OnPropertyChanged(); }
    }

    public int GridStepY
    {
        get => _gridStepY;
        set { _gridStepY = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}