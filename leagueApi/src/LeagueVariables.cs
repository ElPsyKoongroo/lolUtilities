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
    private int ActorCellID;
    private bool banning;
    private List<int> champsToBanId;
    private bool hasBanned;

    public League(){
        _events = new();
        ActorCellID = -1;
        banning = false;
        champsToBanId = new();
        hasBanned = false;
    }
    private TimeSpan getTimeSpanBetween(int start, int end){
            return TimeSpan.FromSeconds( ( Random.Shared.Next(start * 10, end * 10) ) / 10.0 );
    }

    public void addBans(params int[] championsId){
        Array.ForEach(championsId, x=> champsToBanId.Add(x));
    }
}