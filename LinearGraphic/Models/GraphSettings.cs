using System.ComponentModel;

namespace Model;

/// <summary>
/// Настройки графиков с поддержкой уведомлений об изменениях
/// </summary>
public class GraphSettings: INotifyPropertyChanged
{
    /// <summary>
    /// Использовать интерполяцию
    /// </summary>
    [Description("Использовать интерполяцию")]
    public bool Extrapolation { get; set; }

    /// <summary>
    /// Количество отображаемых точек на графике
    /// </summary>
    [Description("Количество точек")]
    public int DisplayPoints { get; set; }

    /// <summary>
    /// Минимальный уровень графика по оси Y
    /// </summary>
    [Description("Минимальный уровень графика по оси Y")]
    public int ChartYLevelMin { get; set; }

    /// <summary>
    /// Максимальный уровень графика по оси Y
    /// </summary>
    [Description("Максимальный уровень графика по оси Y")]
    public int ChartYLevelMax { get; set; }

    /// <summary>
    /// Минимальный уровень графика по оси X
    /// </summary>
    [Description("Минимальный уровень графика по оси X")]
    public int ChartXLevelMin { get; set; }

    /// <summary>
    /// Максимальный уровень графика по оси X
    /// </summary>
    [Description("Максимальный уровень графика по оси X")]
    public int ChartXLevelMax { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
}
