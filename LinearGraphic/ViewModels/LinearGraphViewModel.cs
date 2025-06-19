using Model;
using Core;
using System.Linq;

namespace ViewModels;

public class LinearGraphViewModel
{
    private readonly PointsContext _pointsContext;
    private readonly GraphSettings _graphSettings;
    private readonly IGraphProvider _graphProvider;

    public object GraphControl => _graphProvider.GetGraphControl();

    public LinearGraphViewModel(PointsContext pointsContext, GraphSettings graphSettings, IGraphProvider graphProvider)
    {
        _pointsContext = pointsContext;
        _graphSettings = graphSettings;
        _graphProvider = graphProvider;

        _graphProvider.Initialize(graphSettings);
        _pointsContext.PropertyChanged += (s, e) => UpdateSeries();
        _graphSettings.PropertyChanged += (s, e) => UpdateAxes();
        UpdateAll();
    }

    private void UpdateAll()
    {
        UpdateSeries();
        UpdateAxes();
    }

    private void UpdateSeries()
    {
        _graphProvider.UpdatePoints(_pointsContext.Arr.Take(_graphSettings.DisplayPoints).ToArray());
    }

    private void UpdateAxes()
    {
        _graphProvider.Initialize(_graphSettings);
    }
}
