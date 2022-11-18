using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.Input;
using LeagueUtilities;
using lolClientUtilities.View;

namespace lolClientUtilities.ViewModel;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly League _client;
    private PicknBan? picknBanView;

    private string text = "";
    public string Text
    {
        get => text;
        set { text = value; OnPropertyChange(); }
    }

    private bool autoAccept;
    public bool AutoAccept
    {
        get => autoAccept;
        set { autoAccept = value; OnPropertyChange(); _client.hasToAutoAccept = value; }
    }

    private bool picknBan;
    public bool PicknBan
    { 
        get => picknBan;
        set { picknBan = value; OnPropertyChange(); _client.hasToPick = value; }
    }
    private UserControl actualPage;
    public UserControl ActualPage
    {
        get => actualPage;
        set { actualPage = value; OnPropertyChange(); }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
 
    public void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public MainWindowViewModel()
    {
        _client = League.GetLeague();
    }

    [RelayCommand]
    public async Task Connect()
    {
        await _client.connect();
        _client.hasToPick = picknBan;
        _client.hasToAutoAccept = autoAccept;
    }

    [RelayCommand]
    public void onPicknBanSettings()
    {
        if (ActualPage is not View.PicknBan)
            ActualPage = picknBanView ??= new PicknBan();
        else
            ActualPage = null;
    }
}