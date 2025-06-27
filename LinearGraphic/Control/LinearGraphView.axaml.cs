using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views;

/// <summary>
/// Представление для отображения линейного графика.
/// Содержит базовую логику инициализации пользовательского интерфейса.
/// </summary>
public partial class LinearGraphView : UserControl
{
    /// <summary>
    /// Инициализирует новый экземпляр класса LinearGraphView.
    /// </summary>
    public LinearGraphView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Инициализирует компоненты представления.
    /// </summary>
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}