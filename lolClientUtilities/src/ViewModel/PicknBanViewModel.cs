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
using LeagueUtilities.Models;
using lolClientUtilities.JSON_Classes;
using lolClientUtilities.Model;
using lolClientUtilities.View;
using Log = Serilog.Log;


namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel : INotifyPropertyChanged
{
    private readonly League league;

    //private 
    private List<ChampWithBitmap> allChamps;
    private ObservableCollection<ChampWithBitmap> champsToBan = new();
    private ObservableCollection<ChampWithBitmap> champsToPick = new();
    private ObservableCollection<ChampWithBitmap> champs;
    private string filter = "";

    private bool canPickSkin = false;

    public bool CanPickSkin
    {
        get => canPickSkin;
        set
        {
            canPickSkin = value;
            OnPropertyChange();
        }
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

    private ObservableCollection<ChampWithBitmap> FilterChamps()
    {
        if (filter == "") return new ObservableCollection<ChampWithBitmap>(allChamps);
        var aux = filter.ToLower();

        var champsAux = allChamps.Where(champ => champ.name.ToLower().StartsWith(filter));
        champsAux = champsAux.Concat(allChamps.Where(champ =>
            !champ.name.ToLower().StartsWith(filter) && champ.name.ToLower().Contains(filter)));

        return new ObservableCollection<ChampWithBitmap>(champsAux);
    }

    private async Task<List<Champ>> GetChamps()
    {
        return await league.getAllChamps();
    }

    [RelayCommand]
    private void addPick(ChampWithBitmap champ)
    {
        ChampsToPick.Add(champ);
    }

    [RelayCommand]
    private void remPick(ChampWithBitmap champ)
    {
        ChampsToPick.Remove(champ);
    }

    [RelayCommand]
    private void addBan(ChampWithBitmap champ)
    {
        ChampsToBan.Add(champ);
    }

    [RelayCommand]
    private void remBan(ChampWithBitmap champ)
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
            picks = ChampsToPick.Select(x => x.AsChampJSON()).ToList(),
            bans = ChampsToBan.Select(x => x.AsChampJSON()).ToList()
        };

        string dataSerialized = JsonSerializer.Serialize(data, new JsonSerializerOptions { IncludeFields = true });

        File.WriteAllTextAsync(savesFile, dataSerialized);
    }

    private void Load()
    {
        var savesFile = Path.Combine(Environment.CurrentDirectory, "saves", "PicknBanSaves.json");
        if (!File.Exists(savesFile)) return;

        string data = File.ReadAllText(savesFile);

        PicknBanJSON dataDeserialized =
            JsonSerializer.Deserialize<PicknBanJSON>(data, new JsonSerializerOptions { IncludeFields = true });
        champsToBan =
            new ObservableCollection<ChampWithBitmap>(dataDeserialized.bans.Select(x => new ChampWithBitmap(x, null)));
        champsToPick =
            new ObservableCollection<ChampWithBitmap>(dataDeserialized.picks.Select(x => new ChampWithBitmap(x, null)));
    }

    private async void connect(object? sender, EventArgs e)
    {
        Debug.WriteLine("Conectado");

#if !TEST
        allChamps = (await GetChamps()).Select(x => new ChampWithBitmap(x)).ToList();
        allChamps.RemoveAt(0);
        league.ChampSelectEvent += onChampSelectEvent;
#else
        allChamps = TEST_GetChamps();
#endif
        allChamps = allChamps.OrderBy(x => x.name).ToList();
        Champs = new ObservableCollection<ChampWithBitmap>(allChamps);
        CanPickSkin = true;
        league.ClientConnected -= connect;

        champsToBan = new ObservableCollection<ChampWithBitmap>(
            champsToBan.Select(x => new ChampWithBitmap(x, allChamps.First(y => y.id == x.id).image)).ToList()
        );

        champsToPick = new ObservableCollection<ChampWithBitmap>(
            champsToPick.Select(x => new ChampWithBitmap(x, allChamps.First(y => y.id == x.id).image)).ToList()
        );

        OnPropertyChange(nameof(champsToBan));
        OnPropertyChange(nameof(champsToPick));

        Debug.WriteLine("Terminado de cargar");
    }

    public void onChampSelectEvent(object? sender, EventArgs e)
    {
        List<int> bans = new();
        List<int> picks = new();
        ChampsToBan.ToList().ForEach(ban => bans.Add(ban.id));
        ChampsToPick.ToList().ForEach(pick => picks.Add(pick.id));
        league.SetPicks(bans, picks, OrderComboBox);
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