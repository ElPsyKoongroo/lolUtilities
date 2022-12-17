using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LeagueUtilities.DTO;
using LeagueUtilities.Models;
using lolClientUtilities.Model;
using Microsoft.VisualBasic.Logging;

namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel
{
    private string orderComboBox = "";
    public string OrderComboBox
    {
        get => orderComboBox;
        set { orderComboBox = value; OnPropertyChange(); }
    }
    
    public ObservableCollection<ChampWithBitmap> Champs
    {
        get => champs;
        private set { champs = value; OnPropertyChange(); }
    }
    
    private bool randomSkinChecked = false;
    public bool RandomSkinChecked
    {
        get => randomSkinChecked;
        set { randomSkinChecked = value; OnPropertyChange(); league.HasToPickSkin = value; }
    }
    
    private bool instaPickChecked = false;
    public bool InstaPickChecked
    {
        get => instaPickChecked;
        set { instaPickChecked = value; OnPropertyChange(); league.HasToInstaPick = value; }
    }
    public ObservableCollection<ChampWithBitmap> ChampsToBan { get => champsToBan; }
    public ObservableCollection<ChampWithBitmap> ChampsToPick { get => champsToPick; }
    
    public string Filter
    {
        get => filter;
        set
        {
            if (value == filter) return;
            filter = value;
            OnPropertyChange();
            Champs = FilterChamps();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}