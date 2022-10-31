using LCUSharp.Websocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using LCUSharp;

using LeagueUtilities.DTO;

namespace LeagueUtilities;

public partial class League
{
    Dictionary<string, EventHandler<LeagueEvent>> _events;
    LeagueClientApi? api;

    private void setEvents(){
        if(api is null) return;

        _events.Add("gameflowEvent", new EventHandler<LeagueEvent>(OnGameflowEvent!));
        _events.Add("sessionEvent", new EventHandler<LeagueEvent>(OnSessionEvent!));

        api.Disconnected += Api_Disconnected!;
    }
    //Activate Event every time is needed.
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
        api = await LeagueClientApi.ConnectAsync();

        if(api is null) throw new Exception("No se ha podido conectar al cliente");

        setEvents();

        //Not here
        eventSuscribe("/lol-gameflow/v1/gameflow-phase","gameflowEvent");
        eventSuscribe("/lol-champ-select/v1/session","sessionEvent");

        path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        path = Path.Combine(path, "logs", "file");


        
        // Not here 

        Console.ReadKey();

        disconnect();
        
        Console.ReadKey();
    }
    public void disconnect(){
        if(api is null) return;
        api.Disconnected -= Api_Disconnected!;
        api.Disconnect();
    }

    private async void Api_Disconnected(object sender, EventArgs e)
    {
        if (api is null) return;
        await api.ReconnectAsync();
    }

    
    }