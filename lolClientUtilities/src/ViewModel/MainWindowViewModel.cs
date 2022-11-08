using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LeagueUtilities;
using lolClientUtilities.View;

namespace lolClientUtilities.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    //private League _client = League.GetLeague();

    private string text = "";
    public string Text
    {
        get => text;
        set
        {
            text = value;
            OnPropertyChange();
        }
    }

    private bool autoAccept = false;
    public bool AutoAccept
    {
        get => autoAccept;
        set
        {
            autoAccept = value;
            OnPropertyChange();
        }
    }

    private bool picknBan = false;
    public bool PicknBan
    {
        get => picknBan;
        set
        {
            picknBan = value;
            OnPropertyChange();
        }
    }
    private UserControl actualPage;
    public UserControl ActualPage
    {
        get => actualPage;
        set
        {
            actualPage = value;
            OnPropertyChange();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
 
    public void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public MainWindowViewModel()
    {
        ActualPage = new PicknBan();
    }

}