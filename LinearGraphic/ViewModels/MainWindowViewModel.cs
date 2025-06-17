using Model;

namespace ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    public PointsContext PointsContext { get; }
    public GraphSettings GraphSettings { get; }
    public LinearGraphViewModel LinearGraphViewModel { get; }

    public MainWindowViewModel()
    {
        // Инициализация с некоторыми значениями по умолчанию
        PointsContext = new PointsContext(1000);
        GraphSettings = new GraphSettings
        {
            Extrapolation = true,
            DisplayPoints = 100,
            ChartYLevelMin = 0,
            ChartYLevelMax = 1000,
            ChartXLevelMin = 0,
            ChartXLevelMax = 100
        };

        LinearGraphViewModel = new LinearGraphViewModel(PointsContext, GraphSettings);

        // Добавим тестовые данные
        for (int i = 0; i < 100; i++)
        {
            PointsContext.Add(new Point(i, i * 10));
        }
    }
}