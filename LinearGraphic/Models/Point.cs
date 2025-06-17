namespace Model;

/// <summary>
/// Модель 
/// </summary>
public class Point
{
    /// <summary>
    /// Частота
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Мощность 
    /// </summary>
    public double Y { get; set; }

    public Point(double X, double Y)
    {
        this.X = X;
        this.Y = Y;
    }
}