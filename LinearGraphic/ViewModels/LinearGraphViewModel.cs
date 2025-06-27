using Model;
using Core;
using System.Linq;
using System.Collections.Generic;

namespace ViewModels;

/// <summary>
/// ViewModel для линейного графика, связывающая данные точек с их визуальным представлением.
/// Обеспечивает обновление графика при изменении данных или настроек.
/// </summary>
public class LinearGraphViewModel
{
    private readonly PointsContext _pointsContext;
    private readonly GraphSettings _graphSettings;
    private readonly IGraphProvider _graphProvider;

    /// <summary>
    /// Возвращает элемент управления графиком.
    /// </summary>
    public object GraphControl => _graphProvider.GetGraphControl();

    /// <summary>
    /// Инициализирует новый экземпляр LinearGraphViewModel.
    /// </summary>
    /// <param name="pointsContext">Контекст точек данных.</param>
    /// <param name="graphSettings">Настройки графика.</param>
    /// <param name="graphProvider">Провайдер графиков.</param>
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

    /// <summary>
    /// Обновляет все компоненты графика.
    /// </summary>
    private void UpdateAll()
    {
        UpdateSeries();
        UpdateAxes();
    }

    /// <summary>
    /// Обновляет серии данных на графике.
    /// </summary>
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

    /// <summary>
    /// Обновляет настройки осей графика.
    /// </summary>
    private void UpdateAxes()
    {
        _graphProvider.Initialize(_graphSettings);
    }
}