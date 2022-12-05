using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeagueUtilities.DTO;
using LeagueUtilities.Models;
using lolClientUtilities.Model;

namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel
{
    public ObservableCollection<ChampWithBitmap> Champs
    {
        get => champs;
        private set { champs = value; OnPropertyChange(); }
    }
    
    private bool randomSkinChecked = false;
    public bool RandomSkinChecked
    {
        get => randomSkinChecked;
        set { randomSkinChecked = value; OnPropertyChange(); league.hasToPickSkin = value; }
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