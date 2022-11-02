using Serilog;
using LCUSharp;


using LeagueUtilities.DTO;
using Serilog.Events;

namespace LeagueUtilities;

public partial class League
{
    
    public League()
    {
        CreateLogger();

        _events = new();
        ActorCellID = -1;
        SummonerId = 0;

        prePicks = new();
        bannedAlready = new();
        champsToBanId = new();
        hasBanned = false;

        hasPrepicked = false;
        hasPicked = false;
        champsToPickId = new();
        hasPickSkin = false;

        championId = 0;
    }

    public void addBans(params int[] championsId){
        Array.ForEach(championsId, x=> champsToBanId.Add(x));
    }

    public void addPick(params int[] championsId){
        Array.ForEach(championsId, x=> champsToPickId.Add(x));
    }
    
    private void setEvents(){
        if(api is null) return;
        
        

        _events.Add("gameflowEvent", OnGameflowEvent);
        _events.Add("sessionEvent", OnSessionEvent);

        api.Disconnected += OnDisconnected;
    }

    private void eventSuscribe(string uri, string leagueEvent){
        if(api is null) return;
        api.EventHandler.Subscribe(uri, _events[leagueEvent]);

    }
    // if needed can create a bool array to say if event is setup or not;
    private void eventDesuscribe(string uri){
        if(api is null) return;
        api.EventHandler.Unsubscribe(uri);
    }
    public async Task connect()
    {
        Log.Debug("Contectando con el cliente del lol");
        api = await LeagueClientApi.ConnectAsync();

        if(api is null) throw new Exception("No se ha podido conectar al cliente");
        Log.Debug("Contectado al cliente del lol");

        setEvents();
        await getSummoner();

        //Not here
        eventSuscribe("/lol-gameflow/v1/gameflow-phase","gameflowEvent");
        eventSuscribe("/lol-champ-select/v1/session","sessionEvent");
    }
    public void disconnect(){
        if(api is null) return;
        api.Disconnected -= OnDisconnected!;
        api.Disconnect();
    }
}