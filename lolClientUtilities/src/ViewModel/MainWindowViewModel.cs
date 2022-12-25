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
using LeagueUtilities.DTO;
using lolClientUtilities.View;
using Serilog.Core;
using Serilog;

namespace lolClientUtilities.ViewModel;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly League _client;
    private PicknBan? picknBanView;
    
    private string summonerIconURL = "https://cdn.communitydragon.org/latest/profile-icon/0";
    public string SummonerIconURL
    {
        get => summonerIconURL;
        set { summonerIconURL = value; OnPropertyChange(); }
    }
    
    private SummonerJSON infoSummoner;
    public SummonerJSON InfoSummoner
    {
        get => infoSummoner;
        set { infoSummoner = value; OnPropertyChange(); }
    }
    
    private Visibility stackPanelVisibility;
    public Visibility StackPanelVisibility
    {
        get => stackPanelVisibility; 
        set { stackPanelVisibility = value; OnPropertyChange(); }
    }

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
        set { autoAccept = value; OnPropertyChange(); _client.HasToAutoAccept = value; }
    }

    private bool picknBan;
    public bool PicknBan
    { 
        get => picknBan;
        set
        {
            picknBan = value; 
            OnPropertyChange(); 
            _client.HasToPick = value;
        }
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
        Log.Logger.Debug("Cliente creado");
        stackPanelVisibility = Visibility.Hidden;
        
    }

    [RelayCommand]
    public async Task Connect()
    {
        if (!_client.IsConnected)
        {
            InfoSummoner = await _client.connect();
            SummonerIconURL = $"https://cdn.communitydragon.org/latest/profile-icon/{InfoSummoner.ProfileIconId}";
            _client.HasToPick = picknBan;
            _client.HasToAutoAccept = autoAccept;
            StackPanelVisibility = Visibility.Visible;
        }
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