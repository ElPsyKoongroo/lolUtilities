global using Serilog;
global using Serilog.Events;
global using LCUSharp.Websocket;
global using LCUSharp;
global using System.Linq;
global using System.Text.Json;
global using System.Text.Json.Serialization;

using LeagueUtilities.DTO;

namespace LeagueUtilities;

public partial class League{
    private const int BAN_ACTION = 0;
    private const int BANS_REVEAL_ACTION = 1;
    private const int PICK_ACTION = 2;
    
    private readonly Dictionary<string, EventHandler<LeagueEvent>> _events;
    private readonly ILogger _logC;
    private readonly ILogger _logF;
    LeagueClientApi? api;
    
    
    
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
}