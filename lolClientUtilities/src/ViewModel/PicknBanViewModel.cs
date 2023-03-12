using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CommunityToolkit.Mvvm.Input;
using LeagueUtilities;
using LeagueUtilities.JSON_Classes;
using LeagueUtilities.Models;
using lolClientUtilities.JSON_Classes;
using lolClientUtilities.Model;
using Log = Serilog.Log;


namespace lolClientUtilities.ViewModel;

public partial class PicknBanViewModel : INotifyPropertyChanged
{
    private readonly League league;

    //private 
    private List<ChampWithBitmap> allChamps;
    private ObservableCollection<ChampWithBitmap> champs;
    private ObservableCollection<ChampWithBitmap> champsToBan = new();
    private ObservableCollection<ChampWithBitmap> champsToPick = new();
    private PBProfile actualProfile;
    private string filter = "";
    

    private bool canPickSkin;

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

        var champsAux = allChamps.Where(champ => champ.name.ToLower().StartsWith(aux));
        champsAux = champsAux.Concat(allChamps.Where(champ =>
            !champ.name.ToLower().StartsWith(aux) && champ.name.ToLower().Contains(aux)));

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
        league.ModifyPicks(champsToPick.Select(x=> x.id).ToList());
    }

    [RelayCommand]
    private void remPick(ChampWithBitmap champ)
    {
        ChampsToPick.Remove(champ);
        league.ModifyPicks(champsToPick.Select(x=> x.id).ToList());
    }

    [RelayCommand]
    private void addBan(ChampWithBitmap champ)
    {
        ChampsToBan.Add(champ);
        league.ModifyBans(champsToBan.Select(x=> x.id).ToList());
    }

    [RelayCommand]
    private void remBan(ChampWithBitmap champ)
    {
        ChampsToBan.Remove(champ);
        league.ModifyBans(champsToBan.Select(x=> x.id).ToList());
    }
    
    [RelayCommand]
    public void Save()
    {
        SaveProfile(true);
    }
    
    [RelayCommand]
    public void CreateProfile()
    {
        string baseName = "Profile_";
        int actual = 0;
        
        while (profileComboBoxItems.Contains($"{baseName}{++actual}")){}
        
        PBProfile newProfile = new($"{baseName}{actual}", new(), new());
        ProfileComboBoxItems.Add(newProfile.Name);
        league.SaveProfile(newProfile);
        SelectedProfileName = newProfile.Name;
    }
    
    [RelayCommand]
    public void DeleteProfile()
    {
        if (actualProfile.Name == "main") return;

        if (!league.DeleteProfile(actualProfile)) return; //TODO -> mostrar aviso

        ProfileComboBoxItems.Remove(actualProfile.Name);
        SelectedProfileName = "main";
        actualProfile = league.LoadMainProfile();
    }
    
    private void SaveProfile(bool force = false)
    {
        if (!league.HasProfileChanged(actualProfile,
                ChampWithBitmap.AsChampJSONEnumarable(champsToPick),
                ChampWithBitmap.AsChampJSONEnumarable(champsToBan))) return;
        if(!force)
        {
            var result = MessageBox.Show("Do you want to save your profile?", 
                "Save Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;
        }
        actualProfile.picks = ChampWithBitmap.AsChampJSONEnumarable(champsToPick).ToList();
        actualProfile.bans = ChampWithBitmap.AsChampJSONEnumarable(champsToBan).ToList();
        league.SaveProfile(actualProfile);
           
        MessageBox.Show("Saved successfully",
            "Saved", MessageBoxButtons.OK, MessageBoxIcon.None);
    }
    

    private void LoadProfile()
    {
        if (string.IsNullOrEmpty(SelectedProfileName)) return;
        actualProfile = league.LoadProfile(SelectedProfileName);
        
        champsToBan =
            new ObservableCollection<ChampWithBitmap>(
                actualProfile.bans.Select(x => new ChampWithBitmap(x, null)));
        champsToPick =
            new ObservableCollection<ChampWithBitmap>(
                actualProfile.picks.Select(x => new ChampWithBitmap(x, null)));
        OnPropertyChange(nameof(champsToBan));
        OnPropertyChange(nameof(champsToPick));
    }

    private void Load()
    {
        foreach (var loadProfilesName in league.LoadProfilesNames())
        {
            ProfileComboBoxItems.Add(loadProfilesName);
        }
        actualProfile = league.LoadMainProfile();
        SelectedProfileName = "main";
        
        champsToBan =
            new ObservableCollection<ChampWithBitmap>(
                actualProfile.bans.Select(x => new ChampWithBitmap(x, null)));
        champsToPick =
            new ObservableCollection<ChampWithBitmap>(
                actualProfile.picks.Select(x => new ChampWithBitmap(x, null)));
    }

    private async void connect(object? sender, EventArgs e)
    {
        Debug.WriteLine("Conectado");
        
        allChamps = (await GetChamps()).Select(x => new ChampWithBitmap(x)).ToList();
        allChamps.RemoveAt(0);
        //league.ChampSelectEvent += onChampSelectEvent;
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
        
        league.SetPicksnBans(champsToBan.ToList().Select(x=>x.id).ToList(),
                        champsToPick.Select(x=>x.id).ToList(),
                            orderComboBox);

        league.firstRequest();

    }
}
