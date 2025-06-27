using Model;
using System.Collections.Generic;

namespace Core;

/// <summary>
/// Интерфейс, определяющий провайдер для отображения графиков.
/// Реализации этого интерфейса обеспечивают функциональность отрисовки графиков 
/// с использованием различных базовых технологий.
/// </summary>
public interface IGraphProvider
{
    /// <summary>
    /// Возвращает основной элемент управления графиком, который можно добавить в дерево визуальных элементов.
    /// </summary>
    /// <returns>Объект, представляющий элемент управления графиком.</returns>
    object GetGraphControl();
    
    /// <summary>
    /// Инициализирует провайдер графика с указанными настройками.
    /// </summary>
    /// <param name="settings">Настройки графика, определяющие его внешний вид и поведение.</param>
    void Initialize(GraphSettings settings);
    
    /// <summary>
    /// Обновляет данные для нескольких рядов графика.
    /// </summary>
    /// <param name="series">Словарь, где ключ - имя ряда, а значение - массив точек для отображения.</param>
    void UpdateMultipleSeries(Dictionary<string, Point[]> series);
}