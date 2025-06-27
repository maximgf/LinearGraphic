using System;

namespace Core;

/// <summary>
/// Фабрика для создания экземпляров провайдеров графиков.
/// Обеспечивает централизованную точку создания различных типов графических провайдеров.
/// </summary>
public static class GraphControlFactory
{
    /// <summary>
    /// Перечисление, определяющее доступные типы провайдеров графиков.
    /// </summary>
    public enum GraphProviderType 
    { 
        /// <summary>
        /// Провайдер на основе библиотеки LiveCharts.
        /// </summary>
        LiveCharts, 
        
        /// <summary>
        /// Провайдер с собственной реализацией отрисовки на Canvas.
        /// </summary>
        CustomCanvas
    }
    
    /// <summary>
    /// Создает экземпляр провайдера графика указанного типа.
    /// </summary>
    /// <param name="type">Тип провайдера графика, который требуется создать.</param>
    /// <returns>Экземпляр IGraphProvider соответствующего типа.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если указан неизвестный тип провайдера.</exception>
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