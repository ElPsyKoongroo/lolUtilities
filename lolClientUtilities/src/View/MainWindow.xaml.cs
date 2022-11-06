using System.Windows;
using System.Windows.Input;


/*
 TODO
    Comprobar al desconectarte si es a causa de que el lol se ha cerrado o que se ha entrado en partida // lol-shutdown/v1/notification
 */

namespace lolClientUtilities.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    

    public MainWindow()
    {
        InitializeComponent();
    }

    private void MoverVentana(object sender, MouseButtonEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e){
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    private void onPicknBanSettings(object sender, RoutedEventArgs e){
        
    }
}

