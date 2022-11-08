using LeagueUtilities.DTO;
namespace LeagueUtilities;

public partial class League
{
    private async Task getSummoner(){
        if(api is null ) return;

        var response = await api.RequestHandler
            .GetJsonResponseAsync(HttpMethod.Get,
                "lol-summoner/v1/current-summoner");
        
        if (response is null) return;

        
        var data = JsonSerializer.Deserialize<SummonerJSON>(response, 
            new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            }
        );

        if(data is null) return;

        SummonerId = data.SummonerId;

    }

    public async Task getSelectChampion(){
        if( api is null ) return;

        var response = await api.RequestHandler
            .GetJsonResponseAsync(HttpMethod.Get,
                "lol-champ-select/v1/current-champion");
        
        
        Log.Information(response);
    }

    public async Task acceptGame(){
        if( api is null ) return;
        await Task.Delay(getTimeSpanBetween(1,2));
        var json = await api
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Post,
                "/lol-matchmaking/v1/ready-check/accept",
                Enumerable.Empty<string>());

    }
    
    public async Task<List<ChampsJSON>> getAllChamps(){
        if( api is null ) return new List<ChampsJSON>();

        return await api.RequestHandler
            .GetResponseAsync<List<ChampsJSON>>(HttpMethod.Get,
                $"lol-champions/v1/inventories/{SummonerId}/champions");
        
    }
}