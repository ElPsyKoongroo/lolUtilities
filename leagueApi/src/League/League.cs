using System.Diagnostics;
using System.Reflection.Metadata;
using LeagueUtilities.DTO;

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
        
        handler = new HttpClientHandler();
        
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

        client = new HttpClient(handler);
    }

    public static League GetLeague()
    {
        return _league ??= new League();
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

    public void disconnect(){
        if(api is null) return;
        api.Disconnected -= OnDisconnected;
        api.Disconnect();
    }
}