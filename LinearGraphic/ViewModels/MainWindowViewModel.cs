using Model;
using System;
using System.Threading;

namespace ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PointsContext PointsContext { get; }
    public GraphSettings GraphSettings { get; }
    public LinearGraphViewModel LinearGraphViewModel { get; }

    public MainWindowViewModel()
    {
        PointsContext = new PointsContext(1000);
        GraphSettings = new GraphSettings
        {
            Extrapolation = true,
            DisplayPoints = 1000,
            ChartYLevelMin = -150,
            ChartYLevelMax = 150,
            ChartXLevelMin = 0,
            ChartXLevelMax = 1000
        };

        LinearGraphViewModel = new LinearGraphViewModel(PointsContext, GraphSettings);
    }
}