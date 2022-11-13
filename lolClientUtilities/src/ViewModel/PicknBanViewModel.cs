using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Input;
using LeagueAPI;
using LeagueUtilities;
using LeagueUtilities.DTO;
using lolClientUtilities.View;


namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel : INotifyPropertyChanged
{
    private readonly League league = League.GetLeague();
    private List<ChampsJSON> allChamps;
    private ObservableCollection<ChampsJSON> champsToBan = new();
    private ObservableCollection<ChampsJSON> champsToPick = new();
    private List<ChampsJSON> champs;
    private string filter = "";
    public ICommand addPick { get; set; }
    public ICommand removePick { get; set; }
    public ICommand addBan { get; set; }
    public ICommand removeBan { get; set; }

    public PicknBanViewModel()
    {
        addPick = new RelayCommand<ChampsJSON>(addChampion);
        removePick = new RelayCommand<ChampsJSON>(removeChampion);
        addBan = new RelayCommand<ChampsJSON>(addBanChampion);
        removeBan = new RelayCommand<ChampsJSON>(removeBanChampion);
        connect();
        league.ChampSelectEvent += onChampSelectEvent;
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
    private void addChampion(ChampsJSON champ)
    {
        ChampsToPick.Add(champ);
    }

    private void removeChampion(ChampsJSON champ)
    {
        ChampsToPick.Remove(champ);
    }

    private void addBanChampion(ChampsJSON champ)
    {
        ChampsToBan.Add(champ);
    }
    private void removeBanChampion(ChampsJSON champ)
    {
        ChampsToBan.Remove(champ);
    }

    private async Task connect()
    {
        await league.connect();
        
        

        allChamps = await GetChamps();
        allChamps.RemoveAt(0);
        allChamps = allChamps.OrderBy(x => x.name).ToList();
        Champs = allChamps;
    }
    
    public void onChampSelectEvent(object? sender, EventArgs e)
    {
        List<int> bans = new();
        List<int> picks = new();
        ChampsToBan.ToList().ForEach(ban=>bans.Add(ban.id));
        ChampsToPick.ToList().ForEach(pick=>picks.Add(pick.id));
        league.SetPicks(bans,picks);
        Debug.WriteLine("Fasilito");
    }
}