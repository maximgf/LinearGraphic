using System;
using Avalonia.Threading;
using Core;
using Model;

namespace ViewModels;

/// <summary>
/// ViewModel главного окна приложения.
/// Управляет данными, настройками графика и таймером для обновления данных.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private DispatcherTimer _timer;
    private readonly Random _random = new Random();
    
    /// <summary>
    /// Контекст точек данных с историей и статистикой.
    /// </summary>
    public MultiPointsContext DataPoints { get; }
    
    /// <summary>
    /// Настройки графика.
    /// </summary>
    public GraphSettings GraphSettings { get; }
    
    /// <summary>
    /// ViewModel для линейного графика.
    /// </summary>
    public LinearGraphViewModel LinearGraphViewModel { get; }

    /// <summary>
    /// Инициализирует новый экземпляр MainWindowViewModel.
    /// Создает контекст данных, настройки графика и запускает таймер для обновления данных.
    /// </summary>
    public MainWindowViewModel()
    {
        DataPoints = new MultiPointsContext(1000);
        GraphSettings = new GraphSettings
        {
            Extrapolation = true,
            DisplayPoints = 1000,
            ChartYLevelMin = -150,
            ChartYLevelMax = 150,
            ChartXLevelMin = 0,
            ChartXLevelMax = 1000,
            GridStepX = 100,
            GridStepY = 50
        };

        var graphProvider = GraphControlFactory.CreateProvider(GraphControlFactory.GraphProviderType.CustomCanvas);
        LinearGraphViewModel = new LinearGraphViewModel(DataPoints, GraphSettings, graphProvider);
        
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.1) };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    /// <summary>
    /// Обработчик события таймера. Генерирует новые случайные данные и обновляет график.
    /// </summary>
    private void Timer_Tick(object? sender, EventArgs e)
    {
        Point[] newPoints = new Point[DataPoints.Count];
        int amplitude = 100;

        for (int i = 0; i < newPoints.Length; i++)
        {
            double x = i;
            double y = _random.NextDouble() * 2 * amplitude - amplitude;
            newPoints[i] = new Point(x, y);
        }

        DataPoints.UpdateAllPoints(newPoints);
    }
}