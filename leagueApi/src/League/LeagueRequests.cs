using System.Text.Json;
using Serilog;
using LeagueUtilities.DTO;
namespace LeagueUtilities;

public partial class League
{
    private async Task getSummoner(){
        if(api is null ) return;

        string response = await api.RequestHandler
            .GetJsonResponseAsync(HttpMethod.Get,
                "lol-summoner/v1/current-summoner");
        
        if (response is null) return;

        
        var data = JsonSerializer.Deserialize<SummonerJSON>(response, 
            new JsonSerializerOptions()
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            }
        );

        if(data is null) return;

        SummonerId = data.SummonerId;

    }
    
        private async Task prePick(SessionsJSON sessionData){

        if(champsToPickId.Count == 0) return;

        DTO.Action? prePickAction;
        prePickAction = sessionData.actions[PICK_ACTION]
            .FirstOrDefault( x => x!.actorCellId == ActorCellID, null);

        if( prePickAction is null ) return;        

        var body = new { championId = champsToPickId[0] };

        if(true)
            await Task.Delay(getTimeSpanBetween(1,2));

        var response = await api!
            .RequestHandler
            .GetJsonResponseAsync(HttpMethod.Patch,
            $"/lol-champ-select/v1/session/actions/{prePickAction.id}", 
            Enumerable.Empty<string>(), body);
            
        return;
    }
    private async Task ban(SessionsJSON sessionData)
    {
        if(champsToBanId.Count == 0) return;  

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

            await Task.Delay(getTimeSpanBetween(2,3)); // Podria haber un evento de que ya se haya baneado antes de que se acaben los 2s y no se pueda banear.

            var response = await api! // How to say we checked it before.
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
        
        Array.ForEach( sessionData.myTeam.Where(summoner => summoner.cellId != ActorCellID).ToArray(), myteam => prePicks.Add(myteam.championPickIntent) );
        Array.ForEach( sessionData.bans.myTeamBans.Concat(sessionData.bans.theirTeamBans).ToArray() , teamBans => bannedAlready.Add(teamBans) );

        DTO.Action? pickAction = sessionData.actions[PICK_ACTION]
            .FirstOrDefault( x => x!.actorCellId == ActorCellID, null );

        if(pickAction is null) return;

        foreach(var id in champsToPickId){

            if(prePicks.Contains(id) || bannedAlready.Contains(id)) continue;

            var body = new { championId = id , completed = true};

            championId = id;

            await Task.Delay(getTimeSpanBetween(3,4)); // Podria haber un evento de que ya se haya baneado antes de que se acaben los 2s y no se pueda banear.

            var response = await api! // How to say we checked it before.
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Patch,
                $"/lol-champ-select/v1/session/actions/{pickAction.id}", 
                Enumerable.Empty<string>(), body);
            

            prePicks.Clear();
            bannedAlready.Clear();
            return;
            
        }

        
    }
    public async Task skinPick(){

        System.Console.WriteLine("pickear skin");
        Log.Information("Pick Skin");

        if(api is null ) return;
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
        
        List<int> ownedSkins = skinData.Where(skin => skin.ownership.owned).Select(x=> x.id).ToList();

        var body = new { selectedSkinId =  ownedSkins[ Random.Shared.Next(0,ownedSkins.Count) ] };

        System.Console.WriteLine(body.selectedSkinId);

        await Task.Delay(getTimeSpanBetween(2,3));
        Log.Information("Picking Skin");
        var res = await api.RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Patch,
                        "/lol-champ-select/v1/session/my-selection/",
                        Enumerable.Empty<string>(), body
                        );

    }
}