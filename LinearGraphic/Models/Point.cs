namespace Model;

/// <summary>
/// Модель точки данных с координатами X и Y.
/// Используется для представления данных на графике.
/// </summary>
public class Point
{
    /// <summary>
    /// Координата X точки (обычно представляет частоту).
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Координата Y точки (обычно представляет мощность).
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Инициализирует новый экземпляр точки с указанными координатами.
    /// </summary>
    /// <param name="X">Координата X.</param>
    /// <param name="Y">Координата Y.</param>
    public Point(double X, double Y)
    {
        this.X = X;
        this.Y = Y;
    }
}