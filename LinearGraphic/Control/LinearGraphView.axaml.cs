using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Views;

public partial class LinearGraphView : UserControl
{
    public LinearGraphView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}