using Serilog;
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

    private async void OnGameflowEvent(object? sender, LeagueEvent e)
    {
        if(api is null) return;

        if (e.Data.ToString() == "ReadyCheck")
        {
            await Task.Delay(getTimeSpanBetween(1,2));
            var json = await api
                        .RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Post,
                        "/lol-matchmaking/v1/ready-check/accept",
                        Enumerable.Empty<string>());
        }
    }
    private async void OnSessionEvent(object? sender, LeagueEvent e){
        if ( api is null ) return;

        var sessionData = e.Data.ToObject<SessionsJSON>();
        if (sessionData is null) return;
        
        if(ActorCellID == -1) ActorCellID = sessionData.localPlayerCellId;


        if(sessionData.timer.phase == "PLANNING")
        {
            if(!hasPrepicked){
                hasPrepicked = true;
                await prePick(sessionData);
                return;
            }
        }
        else if (sessionData.timer.phase == "BAN_PICK")
        {
            if(!hasBanned){
                hasBanned = true;
                await ban(sessionData);
                return;
            }
            
            if(!hasPicked && sessionData.actions[BANS_REVEAL_ACTION][0].completed){
                System.Console.WriteLine("PARA PICKEAR");
                hasPicked = true;
                await pick(sessionData);
                return;
            }
        }
        else{
            if(!hasPickSkin){
                hasPickSkin = true;
                await skinPick();
                return;
            }
            
        }
    }
    
    private async void Api_Disconnected(object? sender, EventArgs e)
    {
        if (api is null) return;
        await api.ReconnectAsync();
    }




}