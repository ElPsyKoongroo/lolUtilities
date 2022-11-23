using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LeagueAPI;
using LeagueUtilities;
using LeagueUtilities.DTO;
using lolClientUtilities.JSON_Classes;
using lolClientUtilities.View;
using Log = Serilog.Log;


namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel : INotifyPropertyChanged
{
    private readonly League league;
    //private 
    private List<ChampsJSON> allChamps;
    private ObservableCollection<ChampsJSON> champsToBan = new();
    private ObservableCollection<ChampsJSON> champsToPick = new();
    private ObservableCollection<ChampsJSON> champs;
    private string filter = "";

    private bool canPickSkin = false;

    public bool CanPickSkin
    {
        get => canPickSkin;
        set { canPickSkin = value; OnPropertyChange(); }
    }

    public PicknBanViewModel()
    {
        league = League.GetLeague();

        if (!league.IsConnected)
        {
            Log.Logger.Debug("[PNB VM] No está creado aún el cliente");
            league.ClientConnected += connect;
        }
        else
        {
            Log.Logger.Debug("[PNB VM] Ya está creado el cliente");
            connect(null, EventArgs.Empty);
            CanPickSkin = true;
        }
        Load();
    }
    
    private ObservableCollection<ChampsJSON> FilterChamps()
    {
        if (filter == "") return new ObservableCollection<ChampsJSON>(allChamps);
        var aux = filter.ToLower();

        var champsAux = allChamps.Where(champ => champ.name.ToLower().StartsWith(filter));
        champsAux = champsAux.Concat(allChamps.Where(champ =>
            !champ.name.ToLower().StartsWith(filter) && champ.name.ToLower().Contains(filter)));

        return new ObservableCollection<ChampsJSON>(champsAux);
    }

    private async Task<List<ChampsJSON>> GetChamps()
    {
        return await league.getAllChamps();
    }
    [RelayCommand]
    private void addPick(ChampsJSON champ)
    {
        ChampsToPick.Add(champ);
    }
    [RelayCommand]
    private void remPick(ChampsJSON champ)
    {
        ChampsToPick.Remove(champ);
    }
    [RelayCommand]
    private void addBan(ChampsJSON champ)
    {
        ChampsToBan.Add(champ);
    }
    [RelayCommand]
    private void remBan(ChampsJSON champ)
    {
        ChampsToBan.Remove(champ);
    }

    [RelayCommand]
    private void Save()
    {
        var appPath = Environment.CurrentDirectory;

        var savesDir = Path.Combine(appPath, "saves");
        if (!Directory.Exists(savesDir))
            Directory.CreateDirectory(savesDir);

        var savesFile = Path.Combine(savesDir, "PicknBanSaves.json");

        PicknBanJSON data = new()
        {
            picks = ChampsToPick.ToList(),
            bans = ChampsToBan.ToList()
        };

        string dataSerialized = JsonSerializer.Serialize(data, new JsonSerializerOptions { IncludeFields = true });

        File.WriteAllTextAsync(savesFile, dataSerialized);
    }

    private void Load()
    {
        var savesFile = Path.Combine(Environment.CurrentDirectory, "saves", "PicknBanSaves.json");
        if(!File.Exists(savesFile))return;

        string data = File.ReadAllText(savesFile);

        PicknBanJSON dataDeserialized =
            JsonSerializer.Deserialize<PicknBanJSON>(data, new JsonSerializerOptions { IncludeFields = true });
        champsToBan = new ObservableCollection<ChampsJSON>(dataDeserialized.bans);
        champsToPick = new ObservableCollection<ChampsJSON>(dataDeserialized.picks);
    }

    private async void connect(object? sender, EventArgs e)
    {
        Debug.WriteLine("Conectado");

#if !TEST
        allChamps = await GetChamps();
        allChamps.RemoveAt(0);
        league.ChampSelectEvent += onChampSelectEvent;
#else
        allChamps = TEST_GetChamps();
#endif
        allChamps = allChamps.OrderBy(x => x.name).ToList();
        Champs = new ObservableCollection<ChampsJSON>(allChamps);
        CanPickSkin = true;
        league.ClientConnected -= connect;
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

#region TEST
#if TEST
    public partial class PicknBanViewModel
    {
        private List<ChampsJSON> TEST_GetChamps()
        {
            return new List<ChampsJSON>()
            {
                new ChampsJSON() { alias = "Adri", id = 1, name = "Adrian" },
                new ChampsJSON() { alias = "Eddie", id = 2, name = "Eduard" },
                new ChampsJSON() { alias = "Sargio", id = 3, name = "Sergio" }
            };
        }
    }
#endif
#endregion
    
