using System.Diagnostics;
using System.Reflection.Metadata;
using LeagueUtilities.DB;
using LeagueUtilities.DTO;
using LeagueUtilities.JSON_Classes;

namespace LeagueUtilities;

public partial class League
{
    private static League? _league;
    private League()
    {
        CreateLogger();
        _events = new Dictionary<string, EventHandler<LeagueEvent>>();
        infoSummoner = new();
        SummonerId = 0;

        champsToBanId = new List<int>();
        champsToPickId = new List<int>();
        skinId = 0;
        _phase = PHASES.NONE;
        hasToPick = false;
        hasToPickSkin = false;
        hasToAutoAccept = false;
        IsConnected = false;
        initialized = false;
        orderToPick = "In Order";

        db = createDB();
        handler = new HttpClientHandler();
        
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

        client = new HttpClient(handler);
    }

    public static League GetLeague()
    {
        return _league ??= new League();
    }

    public DBConnection createDB()
    {
        var appPath = Environment.CurrentDirectory;

        var savesDir = Path.Combine(appPath, "data");
        if (!Directory.Exists(savesDir))
            Directory.CreateDirectory(savesDir);

        var savesFile = Path.Combine(savesDir, "save.db");
        return new DBConnection(savesFile);
    }

    public static void Dispose()
    {
        if (_league is null) return;
        
        _league.client.Dispose();
        _league.handler.Dispose();
    }

    public void addBans(params int[] championsId){
        Array.ForEach(championsId, x=> champsToBanId.Add(x));
    }


    public void addPick(params int[] championsId){
        Array.ForEach(championsId, x=> champsToPickId.Add(x));
    }
    
    public void SetPicksnBans(List<int> bans, List<int> picks, string order)
    {
        champsToBanId = bans;
        champsToPickId = picks;
        orderToPick = order;
    }
    
    public void ModifyBans(List<int> bans)
    {
        Debug.WriteLine("Cambiando bans");
        champsToBanId = bans;
        PickBan.ChangeProperty("BanList", bans);
    }
    
    public void ModifyPicks(List<int> picks)
    {
        Debug.WriteLine("Cambiando picks");
        champsToPickId = picks;
        PickBan.ChangeProperty("PickList", picks);
    }

    private void setEvents(){
        if(api is null) return;
        
        _events.Add("gameflowEvent", OnGameflowEvent);

        api.Disconnected += OnDisconnected;
    }

    private void eventSubscribe(string uri, string leagueEvent){
        api?.EventHandler.Subscribe(uri, _events[leagueEvent]);
    }
    // if needed can create a bool array to say if event is setup or not;
    public void eventDesubscribe(string uri){
        api?.EventHandler.Unsubscribe(uri);
    }
    public async Task<SummonerJSON> connect()
    {
        Log.Logger.Debug("Contectando con el cliente del lol");
        api = await LeagueClientApi.ConnectAsync();

        if (api is null)
        {
            var e = new Exception("No se ha podido conectar al cliente");
            Log.Logger.Debug(e, "No se ha podido conectar con el lol");
            throw e;
        }
        
        Log.Logger.Debug("Contectado al cliente del lol");
        await getSummoner();

        if (!initialized)
        {
            setEvents();
            eventSubscribe("/lol-gameflow/v1/gameflow-phase","gameflowEvent");
            initialized = true;
        }
        
        IsConnected = true;
        ClientConnected?.Invoke(this, EventArgs.Empty);
        return infoSummoner;
    }

    public IEnumerable<string> LoadProfilesNames()
    {
        return db
            .GetCollection<PBProfile>(DBConnection.CollectionName.PBProfiles)
            .Select(profile => profile.Name);
    }
    
    public PBProfile LoadProfile(string profileName)
    {
        return db
           .GetCollection<PBProfile>(DBConnection.CollectionName.PBProfiles)
           .First(profile => profile.Name == profileName);
    }
    
    public PBProfile LoadMainProfile()
    {
        var main = db
            .FindEntryByProperty(DBConnection.CollectionName.PBProfiles, (PBProfile p) => p.Name == "main")
            .ToList();
        if(main.Count != 0)
        {
            return main.First();
        }
        db.InsertEntry(DBConnection.CollectionName.PBProfiles, new PBProfile("main", new(), new()));
        main = db
            .FindEntryByProperty(DBConnection.CollectionName.PBProfiles, (PBProfile p) => p.Name == "main")
            .ToList();
        
        if(main.Count == 0)
        {
            throw new Exception("Cant create main profile");
        }
        return main.First();
    }

    public void SaveProfile(PBProfile data)
    {
        db.UpsertEntry<PBProfile>(DBConnection.CollectionName.PBProfiles, data);
    }

    public void DeleteProfile(PBProfile data)
    {
        db.DeleteEntry<PBProfile>(DBConnection.CollectionName.PBProfiles, data.PBProfileId);
    }
    
    public bool HasProfileChanged(PBProfile profile, IEnumerable<ChampsJSON> picks, IEnumerable<ChampsJSON> bans)
    {
        var entry = db.FindEntryById<PBProfile>(DBConnection.CollectionName.PBProfiles, profile.PBProfileId);
        
        var actualPicks = picks.ToList();
        var actualBans = bans.ToList();
        
        if(entry is null)
            return true;
        
        if(actualPicks.Count != entry.picks.Count) return true;
        if(actualBans.Count != entry.bans.Count) return true;
        
        foreach(var pair in actualPicks.Zip(entry.picks))
        {
            if(pair.Item1.id != pair.Item2.id) return true;
        }
        
        foreach(var pair in actualBans.Zip(entry.bans))
        {
            if(pair.Item1.id != pair.Item2.id) return true;
        }
        return false;
    }

    public void disconnect(){
        if(api is null) return;
        api.Disconnected -= OnDisconnected;
        api.Disconnect();
    }
}