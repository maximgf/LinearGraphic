using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

/// <summary>
/// Настройки графиков с поддержкой уведомлений об изменениях
/// </summary>
public class GraphSettings : INotifyPropertyChanged
{
    private bool _extrapolation;
    private int _displayPoints;
    private int _chartYLevelMin;
    private int _chartYLevelMax;
    private int _chartXLevelMin;
    private int _chartXLevelMax;

    /// <summary>
    /// Использовать интерполяцию
    /// </summary>
    [Description("Использовать интерполяцию")]
    public bool Extrapolation
    {
        get => _extrapolation;
        set
        {
            _extrapolation = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Количество отображаемых точек на графике
    /// </summary>
    [Description("Количество точек")]
    public int DisplayPoints
    {
        get => _displayPoints;
        set
        {
            _displayPoints = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Минимальный уровень графика по оси Y
    /// </summary>
    [Description("Минимальный уровень графика по оси Y")]
    public int ChartYLevelMin
    {
        get => _chartYLevelMin;
        set
        {
            _chartYLevelMin = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Максимальный уровень графика по оси Y
    /// </summary>
    [Description("Максимальный уровень графика по оси Y")]
    public int ChartYLevelMax
    {
        get => _chartYLevelMax;
        set
        {
            _chartYLevelMax = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Минимальный уровень графика по оси X
    /// </summary>
    [Description("Минимальный уровень графика по оси X")]
    public int ChartXLevelMin
    {
        get => _chartXLevelMin;
        set
        {
            _chartXLevelMin = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Максимальный уровень графика по оси X
    /// </summary>
    [Description("Максимальный уровень графика по оси X")]
    public int ChartXLevelMax
    {
        get => _chartXLevelMax;
        set
        {
            _chartXLevelMax = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}