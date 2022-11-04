using LeagueUtilities.DTO;
namespace LeagueUtilities;

internal class PickBan
{
    private static PickBan _pickBan = null;
    private LeagueClientApi api;
    private long SummonerId;
    private const int BAN_ACTION = 0;
    private const int BANS_REVEAL_ACTION = 1;
    private const int PICK_ACTION = 2;
    private int ActorCellID;
    private int championId;
    private List<int> champsToBanId;
    private List<int> champsToPickId;
    private bool HasToPicknBan {get; set;}
    private bool hasBanned;
    private bool hasPrepicked;
    private bool hasPicked;
    private bool HasToPickRandomSkin {get; set;}
    private bool hasPickSkin;
    private bool finished;

    private PickBan(LeagueClientApi api, long SummonerId, bool pick, bool skin){
        this.api = api;
        this.SummonerId = SummonerId;

        ActorCellID = -1;

        hasBanned = false;
        hasPrepicked = false;
        hasPicked = false;
        hasPickSkin = false;
        
        HasToPicknBan = pick;
        HasToPickRandomSkin = skin;

        champsToBanId = new();
        champsToPickId = new();

        championId = 0;
        ActorCellID = -1;

        finished = false;
    }

    public static void New(LeagueClientApi api, long SummonerId, bool pick = false, bool skin = false)
    {
        Finish();
        _pickBan = new PickBan(api, SummonerId, pick, skin);
    }

    public static async Task Start()
    {
        var response = await _pickBan.api
            .RequestHandler
            .GetJsonResponseAsync(
                HttpMethod.Get,
                "/lol-champ-select/v1/session"
            );

        var data = JsonSerializer.Deserialize<SessionsJSON>(response, 
            new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            }
        );
        _pickBan.ActorCellID = data!.localPlayerCellId;

        _pickBan.api.EventHandler.Subscribe("/lol-champ-select/v1/session", OnSessionEvent);
        
        await _pickBan.PicknBan(data);
    }

    public static void Finish()
    {
        if (_pickBan is null || _pickBan.finished) return;
        
        _pickBan.finished = true;
        _pickBan.api.EventHandler.Unsubscribe("/lol-champ-select/v1/session");
    }

    private static async void OnSessionEvent(object? sender, LeagueEvent e){

        var sessionData = e.Data.ToObject<SessionsJSON>();
        if (sessionData is null) return;
        
        await _pickBan.PicknBan(sessionData);
        
    }

    private async Task PicknBan(SessionsJSON sessionData)
    {
        switch (sessionData.timer.phase)
        {
            case "PLANNING":
            {
                if(HasToPicknBan && !hasPrepicked){
                    hasPrepicked = true;
                    await prePick(sessionData, ActorCellID);
                }
                break;
            }
            case "BAN_PICK" when HasToPicknBan && !hasBanned:
                hasBanned = true;
                await ban(sessionData);
                return;
            case "BAN_PICK" when HasToPicknBan && !hasPicked && sessionData.actions[BANS_REVEAL_ACTION][0].completed:
                System.Console.WriteLine("PARA PICKEAR");
                hasPicked = true;
                await pick(sessionData);
                return;
            case "BAN_PICK":
                break;
            default:
            {
                if(HasToPickRandomSkin && !hasPickSkin){
                    hasPickSkin = true;
                    await skinPick();
                }
                break;
            }
        }
    } 

    public static void SetPicks(List<int> bans, List<int> picks){
        _pickBan.champsToBanId = bans;
        _pickBan.champsToPickId = picks;
    }


    private async Task prePick(SessionsJSON sessionData, int actorCellId){

    if(champsToPickId.Count == 0) return;

    ActorCellID = actorCellId;

    DTO.Action? prePickAction;
    prePickAction = sessionData.actions[PICK_ACTION]
        .FirstOrDefault( x => x!.actorCellId == ActorCellID, null);

    if( prePickAction is null ) return;        

    var body = new { championId = champsToPickId[0] };

    if(true)
        await Task.Delay(League.getTimeSpanBetween(1,2));

    var response = await api
        .RequestHandler
        .GetJsonResponseAsync(HttpMethod.Patch,
        $"/lol-champ-select/v1/session/actions/{prePickAction.id}", 
        Enumerable.Empty<string>(), body);
        
    return;
    }

    private async Task ban(SessionsJSON sessionData)
    {
        if(champsToBanId.Count == 0) return;

        List<int> prePicks = new();
        List<int> bannedAlready = new();  

        Array.ForEach(sessionData.myTeam, myteam => prePicks.Add(myteam.championPickIntent));
        Array.ForEach(sessionData.bans.myTeamBans, teamBans => bannedAlready.Add(teamBans));

        DTO.Action? banAction;
        banAction = sessionData.actions[BAN_ACTION]
            .FirstOrDefault(
                x => x!.actorCellId == ActorCellID && x.isInProgress,
                null);

        if(banAction is null) return;


        foreach(var id in champsToBanId){

            if(prePicks.Contains(id) || bannedAlready.Contains(id)) continue;

            var body = new { championId = id , completed = true};

            await Task.Delay(League.getTimeSpanBetween(2,3)); // Podria haber un evento de que ya se haya baneado antes de que se acaben los 2s y no se pueda banear.

            var response = await api
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Patch,
                $"/lol-champ-select/v1/session/actions/{banAction.id}", 
                Enumerable.Empty<string>(), body);
            

            prePicks.Clear();
            bannedAlready.Clear();

            return;
            
        }
        
    }

    private async Task pick(SessionsJSON sessionData)
    {
        if(champsToPickId.Count == 0) return;

        List<int> prePicks = new();
        List<int> bannedAlready = new();     
        
        Array.ForEach( sessionData.myTeam.Where(summoner => summoner.cellId != ActorCellID).ToArray(), myteam => prePicks.Add(myteam.championPickIntent) );
        Array.ForEach( sessionData.bans.myTeamBans.Concat(sessionData.bans.theirTeamBans).ToArray() , teamBans => bannedAlready.Add(teamBans) );

        DTO.Action? pickAction = sessionData.actions[PICK_ACTION]
            .FirstOrDefault( x => x!.actorCellId == ActorCellID, null );

        if(pickAction is null) return;

        foreach(var id in champsToPickId){

            if(prePicks.Contains(id) || bannedAlready.Contains(id)) continue;

            var body = new { championId = id , completed = true};

            championId = id;

            await Task.Delay(League.getTimeSpanBetween(3,4)); // Podria haber un evento de que ya se haya baneado antes de que se acaben los 2s y no se pueda banear.

            var response = await api
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Patch,
                $"/lol-champ-select/v1/session/actions/{pickAction.id}", 
                Enumerable.Empty<string>(), body);
            

            prePicks.Clear();
            bannedAlready.Clear();
            return;
            
        }

        
    }

    private async Task skinPick(){

        System.Console.WriteLine("pickear skin");
        Log.Information("Pick Skin");

        Log.Information("Asking for skins");        
        string response = await api.RequestHandler
                            .GetJsonResponseAsync(HttpMethod.Get,
                            $"lol-champions/v1/inventories/{SummonerId}/champions/{championId}/skins");
        
        if (response is null) return;
        
        var skinData = System.Text.Json.JsonSerializer.Deserialize<List<SkinJSON>>(response, 
            new JsonSerializerOptions()
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            }
        );

        if(skinData is null) return;
        
        List<int> ownedSkins = skinData
            .Where(skin => skin.ownership.owned 
                && !skin.isBase
                && !skin.lastSelected)
            .Select(x=> x.id).ToList();

        if(ownedSkins.Count == 0) return;

        var body = new { selectedSkinId =  ownedSkins[Random.Shared.Next(0,ownedSkins.Count)] };

        await Task.Delay(League.getTimeSpanBetween(2,3));
        Log.Information("Picking Skin ",body.selectedSkinId);
        
        var res = await api.RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Patch,
                        "/lol-champ-select/v1/session/my-selection/",
                        Enumerable.Empty<string>(), body
                        );

    }
}