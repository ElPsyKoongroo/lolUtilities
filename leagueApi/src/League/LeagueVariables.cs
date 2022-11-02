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
    
    private readonly Dictionary<string, EventHandler<LeagueEvent>> _events;
    private LeagueClientApi? api;
    private PickBan? pickBan;
    private long SummonerId;
    private List<int> champsToPickId;
    private List<int> champsToBanId;
    private long skinId;

    
}