using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LeagueUtilities;


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
    public static bool test;

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
        LeagueUtilities.League.Dispose();
        Application.Current.Shutdown();
    }
    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e){
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }
}

