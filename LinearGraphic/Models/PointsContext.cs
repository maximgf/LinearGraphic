using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model;

/// <summary>
/// Базовый класс контекста точек, реализующий интерфейс INotifyPropertyChanged.
/// Предоставляет функционал для хранения и обновления массива точек.
/// </summary>
public class PointsContext : INotifyPropertyChanged
{
    /// <summary>
    /// Массив точек данных.
    /// </summary>
    protected Point[] _arr;
    
    /// <summary>
    /// Количество точек в массиве.
    /// </summary>
    protected int _count;

    /// <summary>
    /// Инициализирует новый экземпляр класса PointsContext с указанным размером.
    /// Заполняет массив случайными значениями.
    /// </summary>
    /// <param name="size">Количество точек в массиве.</param>
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

    /// <summary>
    /// Массив точек данных.
    /// </summary>
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

    /// <summary>
    /// Количество точек в массиве.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Обновляет все точки новым массивом точек.
    /// </summary>
    /// <param name="newPoints">Новый массив точек.</param>
    /// <exception cref="ArgumentException">Вызывается, если размер нового массива не совпадает с текущим.</exception>
    public virtual void UpdateAllPoints(Point[] newPoints)
    {
        if (newPoints.Length != _count)
            throw new ArgumentException("Invalid array size");
        
        Array.Copy(newPoints, _arr, _count);
        OnPropertyChanged(nameof(Arr));
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