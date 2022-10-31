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

public partial class League{

    private async void OnGameflowEvent(object sender, LeagueEvent e)
    {
        if(api is null) return;

        /*if (e.Data.ToString() == "ReadyCheck")
        {
            await Task.Delay(getTimeSpanBetween(2,5));
            var json = await api
                        .RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Post,
                        "/lol-matchmaking/v1/ready-check/accept",
                        Enumerable.Empty<string>());
        }*/
    }

    int file = 0;
    string path;

    private async void OnSessionEvent(object sender, LeagueEvent e){
        if ( api is null ) return;

        //var sessionData = e.Data.ToObject<SessionsJSON>();
        //if (sessionData is null) return;

        // Pre pick
        // Ban
        // Pick

        try{
        await File.WriteAllTextAsync($"{path}{file++}.txt", e.Data.ToString());
        }
        catch{}
        
            
        /*
        DTO.Action? accion;

        var ultimaAccion = sessionData.actions.Count() - 1;

        if(ActorCellID == -1){
            ActorCellID = sessionData.localPlayerCellId;
        }

        accion = sessionData.actions[ultimaAccion]
            .FirstOrDefault(
                x => x!.actorCellId == ActorCellID && x.type == "ban" && x.isInProgress,
                null);

        if(accion is null) return;

        */
    
    }

    private async void Ban(SessionsJSON sessionData, DTO.Action accion)
    {
        List<int> prePicks = new();
        List<int> bannedAlready = new();
        
        prePicks.Clear();
        Array.ForEach(sessionData.myTeam, myteam => prePicks.Add(myteam.championPickIntent));
        bannedAlready.Clear();
        Array.ForEach(sessionData.bans.myTeamBans, teamBans => bannedAlready.Add(teamBans));



        if(champsToBanId.Count != 0){    
            foreach(var id in champsToBanId){
                if(prePicks.Contains(id) || bannedAlready.Contains(id)) continue;

                var body = new { championId = id , completed = true};
                
                System.Console.WriteLine($"Se va a banear a {id}");
                await Task.Delay(getTimeSpanBetween(1,2));
                var response = await api
                    .RequestHandler
                    .GetJsonResponseAsync(HttpMethod.Patch,
                    $"/lol-champ-select/v1/session/actions/{accion.id}", 
                    Enumerable.Empty<string>(), body);
                
                break;
                
            }


        }
    }
}