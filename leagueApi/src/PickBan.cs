using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LeagueUtilities.DTO;
namespace LeagueUtilities;

internal class PickBan
{
    private static PickBan? _pickBan;
    private readonly LeagueClientApi api;
    private readonly long SummonerId;
    private int ActorCellID;
    private int championId;
    private List<int> champsToBanId;
    private List<int> champsToPickId;
    private bool HasToPicknBan {get; set;}
    private bool hasBanned;
    private bool hasPrepicked;
    private bool hasPicked;
    private bool HasToInstaPick;
    private bool HasToPickRandomSkin {get; set;}
    private bool hasPickSkin;
    private bool finished;
    private int PickPosition;
    private int BanPosition;
    private Dictionary<string, List<ChampsJSON>> PickBanProfile;

    private string orderToPick;

    private PickBan(LeagueClientApi api, long SummonerId, bool pick, bool skin, bool instaPick){
        this.api = api;
        this.SummonerId = SummonerId;

        ActorCellID = -1;

        hasBanned = false;
        hasPrepicked = false;
        hasPicked = false;
        hasPickSkin = false;
        
        HasToPicknBan = pick;
        HasToPickRandomSkin = skin;
        HasToInstaPick = instaPick;

        champsToBanId = new List<int>();
        champsToPickId = new List<int>();

        championId = 0;
        ActorCellID = -1;

        PickPosition = -1;
        BanPosition = -1;

        finished = false;
        orderToPick = "In Order";
    }
    public static void New(LeagueClientApi api, long SummonerId, bool pick = false, bool skin = false, bool instaPick = false)
    {
        Finish();
        _pickBan = new PickBan(api, SummonerId, pick, skin, instaPick);
        Log.Logger.Debug(
            "Creado nuevo objeto PicknBan:\n" +
            "\tPick: {@Pick}\n" +
            "\tInstaPick: {@InstaPick}\n" +
            "\tSkin: {@Skin}",
            pick, instaPick, skin);
    }
    public static async Task Start(bool reconnect=false)
    {
        var data = await _pickBan!.api
            .RequestHandler
            .GetResponseAsync<SessionsJSON>(
                HttpMethod.Get,
                "/lol-champ-select/v1/session"
            );

        _pickBan.ActorCellID = data!.localPlayerCellId;
        Log.Logger.Debug("ActorCellID: {@ActorCellID}", _pickBan.ActorCellID);

        _pickBan.SearchIndex(data);
        if(!reconnect)
            _pickBan.api.EventHandler.Subscribe("/lol-champ-select/v1/session", OnSessionEvent);
        
        await _pickBan.PicknBan(data);
    }

    public static void ChangeProperty(string propName, object value)
    {
        if (_pickBan is null || _pickBan.finished) return;
        Debug.WriteLine($"Changing {propName}");
        switch (propName)
        {
            case "HasToPicknBan":
            {
                var propValue = (bool)value;

                _pickBan.HasToPicknBan = propValue;

                if (propValue)
                {
                    Start(true);
                }
                break;
            }
                
            case "HasToPickRandomSkin":
            {
                var propValue = (bool)value;

                _pickBan.HasToPickRandomSkin = propValue;
                break;
            }
            
            case "BanList":
            {
                var propValue = (List<int>)value;

                _pickBan.champsToBanId = propValue;
                break;
            }
            
            case "PickList":
            {
                var propValue = (List<int>)value;

                _pickBan.champsToPickId = propValue;
                break;
            }

            case "HasToInstaPick":
            {
                var propValue = (bool)value;

                _pickBan.HasToInstaPick = propValue;
                break;
            }
            
            default:
            {
                Debug.WriteLine("Cagaste si has entrado aqui");
                break;
            }
        }
    }

    private static bool IsConnected()
    {
        return !(_pickBan is null || _pickBan.finished);
    }
    private void SearchIndex(SessionsJSON data)
    {
        for (var i = 0; i < data.actions.Length; i++)
        {
            var action = data.actions[i];
            if(action.Length == 0) continue;

            switch (action[0].type)
            {
                case "ban" when action.Any(player=> player.actorCellId == ActorCellID):
                    BanPosition = i;
                    Log.Logger.Debug("BanPosition: {@BanPosition}", i);
                    if (PickPosition != -1) return;
                    break;
                case "pick" when action.Any(player=> player.actorCellId == ActorCellID):
                    PickPosition = i;
                    Log.Logger.Debug("PickPosition: {@PickPosition}", i);
                    if (BanPosition != -1) return;
                    break;
            }
        }
    }
    public static void Finish()
    {
        if (!IsConnected()) return;
        
        _pickBan.finished = true;
        _pickBan.api.EventHandler.Unsubscribe("/lol-champ-select/v1/session");
        Log.Logger.Debug("PicknBan Terminado");
    }
    private static async void OnSessionEvent(object? sender, LeagueEvent e){

        var sessionData = e.Data.ToObject<SessionsJSON>();
        if (sessionData is null) return;
        
        await _pickBan.PicknBan(sessionData);
    }
    private async Task PicknBan(SessionsJSON sessionData)
    {
        try
        {
            switch (sessionData.timer.phase)
            {
                case "PLANNING":
                    if (HasToPicknBan && !hasPrepicked)
                    {
                        hasPrepicked = true;
                        Log.Logger.Debug("Entrando a PrePick");
                        await prePick(sessionData);
                    }

                    break;
                case "BAN_PICK" when HasToPicknBan:
                {
                    if (!hasBanned)
                    {
                        if (BanPosition == -1) SearchIndex(sessionData);
                        if (BanPosition != -1)
                        {
                            if (sessionData.actions[BanPosition].Any(
                                    player => player.actorCellId == ActorCellID
                                              && player.isInProgress))
                            {
                                hasBanned = true;
                                Log.Logger.Debug("Entrando a Ban");
                                await ban(sessionData);
                            }
                        }
                    }

                    if (hasPicked) return;
                    if (PickPosition == -1) SearchIndex(sessionData);
                    if (PickPosition == -1) return;
                    
                    if (!sessionData.actions[PickPosition].Any(
                            player => player.actorCellId == ActorCellID
                                      && player.isInProgress)) return;
                    
                    hasPicked = true;
                    Log.Logger.Debug("Entrando a Pick");
                    await pick(sessionData);

                    return;
                }
                case "BAN_PICK":
                    break;
                default:
                    if (HasToPickRandomSkin && !hasPickSkin)
                    {
                        hasPickSkin = true;
                        Log.Logger.Debug("Entrando a PickSkin");
                        await skinPick();
                    }

                    break;
            }
        }
        catch (Exception e)
        {
            Log.Logger.Debug(e, "ERROR:");
        }
    }
    public static void SetPicks(List<int> bans, List<int> picks,string order){
        _pickBan!.champsToBanId = bans;
        _pickBan.champsToPickId = picks;
        _pickBan.orderToPick = order;
        switch (_pickBan.orderToPick)
        {
            case "Random":
            {
                Random rand = new();
                _pickBan.champsToPickId = _pickBan.champsToPickId.OrderBy(c => rand.Next()).ToList();
                break;
            }
            case "In Order":
            {
                break;
            }
        }
    }
    private async Task prePick(SessionsJSON sessionData)
    {
        
        var champsToPick = champsToPickId.Count;
        Log.Logger.Debug("Champs to PrePick: {@PrePicks}", champsToPick);
        
        if (champsToPick == 0)
        {
            Log.Logger.Debug("No se puede PrePickear nada");
            return;
        }
        
        if(champsToPickId.Count == 0) return;

        var prePickAction = sessionData.actions[PickPosition]
            .FirstOrDefault( x => x!.actorCellId == ActorCellID, null);

        if( prePickAction is null ) return;        

        var body = new { championId = champsToPickId[0] };
    
        var time = League.getTimeSpanBetween(9, 10);
        Log.Logger.Debug("Esperando {@Tiempo}", time);
        await Task.Delay(time);

        var response = await api
            .RequestHandler
            .GetJsonResponseAsync(HttpMethod.Patch,
            $"/lol-champ-select/v1/session/actions/{prePickAction.id}", 
            Enumerable.Empty<string>(), body);
    
        Log.Logger.Debug("PrePick de {@Champ}", champsToPickId[0]);
    }
    private async Task ban(SessionsJSON sessionData)
    {
        var champsToBan = champsToBanId.Count;
        Log.Logger.Debug("Champs to Ban: {@Bans}", champsToBan);
        
        if (champsToBan == 0)
        {
            Log.Logger.Debug("No se puede banear nada");
            return;
        }

        List<int> prePicks = new();
        List<int> bannedAlready = new();  

        Array.ForEach(sessionData.myTeam, myteam => prePicks.Add(myteam.championPickIntent));
        Array.ForEach(sessionData.bans.myTeamBans, teamBans => bannedAlready.Add(teamBans));

        var banAction = sessionData.actions[BanPosition]
            .FirstOrDefault(
                x => x!.actorCellId == ActorCellID && x.isInProgress,
                null);

        if(banAction is null) return;
        
        var time = League.getTimeSpanBetween(2, 3);
        Log.Logger.Debug("Esperando {@Tiempo}", time);
        await Task.Delay(time);
    

        foreach(var id in champsToBanId){

            if(prePicks.Contains(id) || bannedAlready.Contains(id)) continue;

            var body = new { championId = id , completed = true};


            Log.Logger.Debug("Baneando a {@ID}", id);
            try
            {
                await api
                    .RequestHandler
                    .GetJsonResponseAsync(HttpMethod.Patch,
                        $"/lol-champ-select/v1/session/actions/{banAction.id}",
                        Enumerable.Empty<string>(), body);
                Log.Logger.Debug("{@ID} baneado", id);
                return;
            }
            catch(Exception e)
            {
                Log.Logger.Debug(e, "No se ha podido banear a {@ID}", id);
            }
        }
        prePicks.Clear();
        bannedAlready.Clear();
    }
    private async Task pick(SessionsJSON sessionData)
    {
        var champsToPick = champsToPickId.Count;
        Log.Logger.Debug("Champs to Pick: {@Pick}", champsToPick);

        if (champsToPick == 0)
        {
            Log.Logger.Debug("No se puede pickear nada");
            return;
        }

        List<int> prePicks = new();
        List<int> bannedAlready = new();     
        
        Array.ForEach( sessionData.bans.myTeamBans.Concat(sessionData.bans.theirTeamBans).ToArray() , teamBans => bannedAlready.Add(teamBans) );

        Log.Logger.Debug("Getting pick action");
        
        var pickAction = sessionData.actions[PickPosition]
            .FirstOrDefault( x => x!.actorCellId == ActorCellID, null );
        if(pickAction is null) return;
        
        Log.Logger.Debug("Pick action ID: {@ID}", pickAction.id);
        
        if (!HasToInstaPick)
        {
            var time = League.getTimeSpanBetween(3, 4);
            Log.Logger.Debug("Esperando {@Tiempo}", time);
            await Task.Delay(time);
        }
        else Log.Logger.Debug("Instapickeando");
    

        foreach(var id in champsToPickId)
        {
            Log.Logger.Debug("Intentando pickear a {@Champ}", id);

            if (bannedAlready.Contains(id))
            {
                Log.Logger.Debug("Already banned");
                continue;
            }

            var body = new { championId = id , completed = true};
            
            Log.Logger.Debug("Pickeando a {@Champ}", id);
            try
            {
                await api
                    .RequestHandler
                    .GetJsonResponseAsync(HttpMethod.Patch,
                        $"/lol-champ-select/v1/session/actions/{pickAction.id}",
                        Enumerable.Empty<string>(), body);
                Log.Logger.Debug("{@Champ} pickeado", id);
                
                championId = id;
                return;
            }
            catch (Exception e)
            {
                Log.Logger.Debug(e, "No se ha podido pickear a {@Champ}", id);
            }
        }

        
    }
    private async Task skinPick()
    {
        Log.Logger.Debug("Pick Skin");

        Log.Logger.Debug("Asking for skins");        
        var skinData = await api.RequestHandler
                            .GetResponseAsync<List<SkinJSON>>(HttpMethod.Get,
                            $"lol-champions/v1/inventories/{SummonerId}/champions/{championId}/skins");
        
        if (skinData is null) return;

        //TODO => si no 
        //if (notBaseSkin)
        //    if (skinData.First(x => x.isBase).id == skinData.First(x => x.lastSelected).id) return;

            var ownedSkins = skinData
            .Where(skin => skin.ownership.owned 
                           && skin is { isBase: false, lastSelected: false })
            .Select(x=> x.id).ToList();
        
        Log.Logger.Debug("Total skins: {@NSkins}");
        
        if(ownedSkins.Count == 0) return;

        var body = new { selectedSkinId =  ownedSkins[Random.Shared.Next(0,ownedSkins.Count)] };

        await Task.Delay(League.getTimeSpanBetween(2,3));
        Log.Logger.Debug("Picking Skin {@Skin}",body.selectedSkinId);
        
        await api.RequestHandler
            .GetJsonResponseAsync(HttpMethod.Patch,
                "/lol-champ-select/v1/session/my-selection/",
                Enumerable.Empty<string>(), body
            );
        Log.Logger.Debug("Skin {@Skin} picked",body.selectedSkinId);
    }
}