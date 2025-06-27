using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

/// <summary>
/// Класс настроек графика, реализующий интерфейс INotifyPropertyChanged.
/// Содержит параметры отображения графика, включая пределы осей, шаг сетки и другие настройки.
/// </summary>
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

    /// <summary>
    /// Флаг, указывающий, следует ли использовать экстраполяцию данных.
    /// </summary>
    public bool Extrapolation
    {
        get => _extrapolation;
        set { _extrapolation = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Количество точек для отображения на графике.
    /// </summary>
    public int DisplayPoints
    {
        get => _displayPoints;
        set { _displayPoints = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Минимальное значение оси Y.
    /// </summary>
    public int ChartYLevelMin
    {
        get => _chartYLevelMin;
        set { _chartYLevelMin = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Максимальное значение оси Y.
    /// </summary>
    public int ChartYLevelMax
    {
        get => _chartYLevelMax;
        set { _chartYLevelMax = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Минимальное значение оси X.
    /// </summary>
    public int ChartXLevelMin
    {
        get => _chartXLevelMin;
        set { _chartXLevelMin = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Максимальное значение оси X.
    /// </summary>
    public int ChartXLevelMax
    {
        get => _chartXLevelMax;
        set { _chartXLevelMax = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Шаг сетки по оси X.
    /// </summary>
    public int GridStepX
    {
        get => _gridStepX;
        set { _gridStepX = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Шаг сетки по оси Y.
    /// </summary>
    public int GridStepY
    {
        get => _gridStepY;
        set { _gridStepY = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Событие, возникающее при изменении свойств.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Вызывает событие PropertyChanged при изменении свойства.
    /// </summary>
    /// <param name="propertyName">Имя измененного свойства.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}