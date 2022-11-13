using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeagueUtilities.DTO;

namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel
{
    public List<ChampsJSON> Champs
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

    public ObservableCollection<ChampsJSON> ChampsToBan { get => champsToBan; }
    public ObservableCollection<ChampsJSON> ChampsToPick { get => champsToPick; }
    
    public string Filter
    {
        get => filter;
        set
        {
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