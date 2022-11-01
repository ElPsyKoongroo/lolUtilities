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
    private const int BAN_ACTION = 0;
    private const int BANS_REVEAL_ACTION = 1;
    private const int PICK_ACTION = 2;
    private int ActorCellID;
    private long SummonerId;
    //Pick_and_Ban
    private bool hasBanned;
    private List<int> champsToBanId;
    List<int> prePicks;
    List<int> bannedAlready;
    private bool hasPrepicked;
    private bool hasPicked;
    private bool hasPickSkin;
    List<int> champsToPickId;
    // Current
    int championId;

    public League(){

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

    private void CreateLogger()
    {
        const string formato = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: formato
                )
            //.Filter.ByExcluding(Matching.WithProperty<int>("Count", p => p < 5))
            .CreateLogger();
    }

    private TimeSpan getTimeSpanBetween(int start, int end){
            return TimeSpan.FromSeconds( ( Random.Shared.Next(start * 10, end * 10) ) / 10.0 );
    }

    public void addBans(params int[] championsId){
        Array.ForEach(championsId, x=> champsToBanId.Add(x));
    }

    public void addPick(params int[] championsId){
        Array.ForEach(championsId, x=> champsToPickId.Add(x));
    }
}