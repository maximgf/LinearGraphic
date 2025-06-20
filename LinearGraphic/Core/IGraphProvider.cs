using Model;
using System.Collections.Generic;

namespace Core;

public interface IGraphProvider
{
    object GetGraphControl();
    void Initialize(GraphSettings settings);
    void UpdateMultipleSeries(Dictionary<string, Point[]> series);
}