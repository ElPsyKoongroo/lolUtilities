global using Serilog;
global using Serilog.Events;
global using LCUSharp.Websocket;
global using LCUSharp;
global using System.Linq;
global using System.Text.Json;
using LeagueUtilities.DTO;
using LeagueUtilities.DB;

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
    private DBConnection db;
    public bool IsConnected { get; private set; }
    private LeagueClientApi? api;
    private long SummonerId;
    private List<int> champsToPickId;
    private List<int> champsToBanId;
    private long skinId;
    public EventHandler ChampSelectEvent;
    public EventHandler ClientConnected;
    private PHASES _phase;
    private bool initialized;
    private string orderToPick;
    public SummonerJSON infoSummoner { get; private set; }

    private readonly HttpClientHandler handler;

    private readonly HttpClient client;
    //Temporal
    private bool hasToPick;
    private bool hasToPickSkin;
    private bool hasToInstaPick;
    private bool hasToAutoAccept;
    
    #region ObservableVariables
    public bool HasToPick
    {
        get => hasToPick;
        set
        {
            hasToPick = value;
            PickBan.ChangeProperty("HasToPicknBan", value);
        }
    }

    public bool HasToPickSkin
    {
        get => hasToPickSkin;
        set
        {
            hasToPickSkin = value;
            PickBan.ChangeProperty("HasToPickRandomSkin", value);
        }
    }

    public bool HasToInstaPick
    {
        get => hasToInstaPick;
        set
        {
            hasToInstaPick = value;
            PickBan.ChangeProperty("HasToInstaPick", value);
        }
    }

    public bool HasToAutoAccept
    {
        get => hasToAutoAccept;
        set => hasToAutoAccept = value;
    }

    private PHASES phase
    {
        get => _phase;
        set
        {
            _phase = value;
            if (_phase == PHASES.CHAMPSELECT)
            {
                //onChampSelect();
            }
        }
    }
    #endregion

    public void onChampSelect()
    {
        ChampSelectEvent.Invoke(this, EventArgs.Empty);
    }

    
}