using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using LeagueAPI;
using LeagueUtilities;
using LeagueUtilities.DTO;

namespace lolClientUtilities.ViewModel;

public class PicknBanViewModel : INotifyPropertyChanged
{
    private List<ChampsJSON> champs = GetChamps();
    public List<ChampsJSON> Champs
    {
        get => champs;
        set
        {
            champs = value;
            OnPropertyChange();
        }
    }

    private static List<ChampsJSON> GetChamps()
    {
        return new List<ChampsJSON>()
        {
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Adrian" },
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Eddie" },
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Sergio" },
        };
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
 
    public void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}