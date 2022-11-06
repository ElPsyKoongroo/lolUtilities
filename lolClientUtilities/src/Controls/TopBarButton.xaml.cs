using System.Windows;
using System.Windows.Controls;

namespace lolClientUtilities.Controls;

public partial class TopBarButton : UserControl
{
    
    public static readonly DependencyProperty ImgSourceProperty = 
        DependencyProperty.Register("ImgSource", typeof(string), typeof(TopBarButton)
        , new PropertyMetadata(string.Empty));

    public string ImgSource
    {
        get => (string)GetValue(ImgSourceProperty);
        set => SetValue(ImgSourceProperty, value);
    }
    
    public TopBarButton()
    {
        InitializeComponent();
    }
}