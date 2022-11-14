global using Serilog;
global using Serilog.Events;
global using LCUSharp.Websocket;
global using LCUSharp;
global using System.Linq;
global using System.Text.Json;

namespace LeagueUtilities;

public partial class League{

    private enum PHASES
    {
        NONE,
        READYCHECK,
        LOBBY,
        MATCHMAKING,
        CHAMPSELECT,
        INPROGRESS,
        WAITINGFORSTATS,
        ENDOFGAME,
    }
    
    private readonly Dictionary<string, EventHandler<LeagueEvent>> _events;
    public bool IsConnected { get; private set; }
    private LeagueClientApi? api;
    private long SummonerId;
    private List<int> champsToPickId;
    private List<int> champsToBanId;
    private long skinId;
    public EventHandler ChampSelectEvent;
    public EventHandler ClientConnected;
    private PHASES _phase;
    //Temporal
    public bool hasToPick { get; set; }
    public bool hasToPickSkin { get; set; }
    public bool hasToAutoAccept { get; set; }

    private PHASES phase
    {
        get => _phase;
        set
        {
            _phase = value;
            if (_phase == PHASES.CHAMPSELECT)
            {
                onChampSelect();
            }
        }
    }

    public void onChampSelect()
    {
        ChampSelectEvent.Invoke(this, EventArgs.Empty);
    }

    
}