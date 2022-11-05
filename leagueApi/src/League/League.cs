namespace LeagueUtilities;

public partial class League
{
    
    public League()
    {
        CreateLogger();
        _events = new Dictionary<string, EventHandler<LeagueEvent>>();
        SummonerId = 0;

        champsToBanId = new List<int>();
        champsToPickId = new List<int>();
        skinId = 0;


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

        api.Disconnected += OnDisconnected;
    }

    private void eventSubscribe(string uri, string leagueEvent){
        api?.EventHandler.Subscribe(uri, _events[leagueEvent]);
    }
    // if needed can create a bool array to say if event is setup or not;
    public void eventDesubscribe(string uri){
        api?.EventHandler.Unsubscribe(uri);
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
        eventSubscribe("/lol-gameflow/v1/gameflow-phase","gameflowEvent");
        //eventSuscribe("/lol-champ-select/v1/session","sessionEvent");
    }
    public void disconnect(){
        if(api is null) return;
        api.Disconnected -= OnDisconnected;
        api.Disconnect();
    }
}