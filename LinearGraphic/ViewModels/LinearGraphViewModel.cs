using Model;
using Core;
using System.Linq;
using System.Collections.Generic;

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
        var series = new Dictionary<string, Point[]>();
        
        series["Common"] = _pointsContext.Arr.Take(_graphSettings.DisplayPoints).ToArray();
        
        if (_pointsContext is MultiPointsContext multiContext)
        {
            if (multiContext.MaxPoints != null)
                series["Max"] = multiContext.MaxPoints.Take(_graphSettings.DisplayPoints).ToArray();
            
            if (multiContext.MinPoints != null)
                series["Min"] = multiContext.MinPoints.Take(_graphSettings.DisplayPoints).ToArray();
            
            if (multiContext.AvgPoints != null)
                series["Average"] = multiContext.AvgPoints.Take(_graphSettings.DisplayPoints).ToArray();
        }
        
        _graphProvider.UpdateMultipleSeries(series);
    }

    private void UpdateAxes()
    {
        _graphProvider.Initialize(_graphSettings);
    }
}