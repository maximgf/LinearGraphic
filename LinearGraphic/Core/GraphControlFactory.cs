using System;

namespace Core;

public static class GraphControlFactory
{
    public enum GraphProviderType { LiveCharts, CustomCanvas }

    public static IGraphProvider CreateProvider(GraphProviderType type)
    {
        return type switch
        {
            GraphProviderType.LiveCharts => new LiveChartsProvider(),
            GraphProviderType.CustomCanvas => new CustomCanvasProvider(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}