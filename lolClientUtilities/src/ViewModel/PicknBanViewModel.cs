using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using LeagueAPI;
using LeagueUtilities;
using LeagueUtilities.DTO;

namespace lolClientUtilities.ViewModel;

public class PicknBanViewModel : INotifyPropertyChanged
{
    private League league = League.GetLeague();
    private List<ChampsJSON> allChamps;

    private List<ChampsJSON> champs;
    public List<ChampsJSON> Champs
    {
        get => champs;
        set
        {
            champs = value;
            OnPropertyChange();
        }
    }
    private string filter;
    public string Filter
    {
        get => filter;
        set
        {
            filter = value;
            OnPropertyChange();
            //Debug.WriteLine(filter);
            //Champs = FilterChamps();
        }
    }
    

    private List<ChampsJSON> FilterChamps()
    {
        return filter == "" ? allChamps : allChamps.Where(champ => champ.name.Contains(filter) || champ.alias.Contains(filter)).ToList();
    }

    private async Task<List<ChampsJSON>> GetChamps()
    {
        return await league.getAllChamps();
        /*return new List<ChampsJSON>()
        {
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Adrian" },
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Eddie" },
            new ChampsJSON() { alias = "Adrian", id = 1, name = "Sergio" },
        };*/
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChange([CallerMemberName]string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public PicknBanViewModel()
    {
        connect();
    }

    private async Task connect()
    {
        
        await league.connect();

        allChamps = await GetChamps();
        allChamps.RemoveAt(0);
        Champs = allChamps;
        
        champs.Clear();
    }
}