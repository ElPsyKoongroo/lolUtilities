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
}