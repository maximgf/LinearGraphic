using Model;
using System;
using Avalonia.Threading;

namespace ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private DispatcherTimer _timer;
    private double _phase;
    public PointsContext DataPoints { get; }
    public GraphSettings GraphSettings { get; }
    public LinearGraphViewModel LinearGraphViewModel { get; }

    public MainWindowViewModel()
    {
        DataPoints = new PointsContext(1000);
        GraphSettings = new GraphSettings
        {
            Extrapolation = true,
            DisplayPoints = 1000,
            ChartYLevelMin = -150,
            ChartYLevelMax = 150,
            ChartXLevelMin = 0,
            ChartXLevelMax = 1000
        };
 
        LinearGraphViewModel = new LinearGraphViewModel(DataPoints, GraphSettings);
        
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(0.1);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _phase += 0.1;
        
        Point[] newPoints = new Point[DataPoints.Count];
        double amplitude = 100;
        double periods = 4;
        double step = 2 * Math.PI * periods / newPoints.Length;

        for (int i = 0; i < newPoints.Length; i++)
        {
            double x = i;
            double y = amplitude * Math.Sin(i * step + _phase);
            newPoints[i] = new Point(x, y);
        }

        DataPoints.UpdateAllPoints(newPoints);
    }
}