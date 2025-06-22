using System;
using Avalonia.Threading;
using Core;
using Model;

namespace ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private DispatcherTimer _timer;
    private readonly Random _random = new Random();
    public MultiPointsContext DataPoints { get; }
    public GraphSettings GraphSettings { get; }
    public LinearGraphViewModel LinearGraphViewModel { get; }

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

    private void Timer_Tick(object? sender, EventArgs e)
    {
        Point[] newPoints = new Point[DataPoints.Count];
        int amplitude = 100; // Амплитуда случайных значений

        for (int i = 0; i < newPoints.Length; i++)
        {
            double x = i;
            double y = _random.NextDouble() * 2 * amplitude - amplitude; // Случайное значение от -amplitude до +amplitude
            newPoints[i] = new Point(x, y);
        }

        DataPoints.UpdateAllPoints(newPoints);
    }
}