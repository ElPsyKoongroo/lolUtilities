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
    private readonly League league = League.GetLeague();
    private List<ChampsJSON> allChamps;

    private List<ChampsJSON> champs;
    public List<ChampsJSON> Champs
    {
        get => champs;
        private set
        {
            champs = value;
            OnPropertyChange();
        }
    }
    private string filter = "";

    public string Filter
    {
        get => filter;
        set
        {
            filter = value;
            OnPropertyChange();
            //Debug.WriteLine(filter);
            Champs = FilterChamps();
        }
    }
    

    private List<ChampsJSON> FilterChamps()
    {
        if (filter == "") return allChamps;
        var aux = filter.ToLower();

        var champsAux = allChamps.Where(champ => champ.name.ToLower().StartsWith(filter));
        champsAux = champsAux.Concat(allChamps.Where(champ =>
            !champ.name.ToLower().StartsWith(filter) && champ.name.ToLower().Contains(filter)));

        return champsAux.ToList();
    }

    private async Task<List<ChampsJSON>> GetChamps()
    {
        return await league.getAllChamps();
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
        allChamps = allChamps.OrderBy(x => x.name).ToList();
        Champs = allChamps;
    }
}