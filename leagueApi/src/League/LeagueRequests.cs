using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using LeagueUtilities.DTO;
using LeagueUtilities.Models;

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
        
        Log.Logger.Debug(response);
    }

    private async Task acceptGame(){
        if( api is null ) return;
        await Task.Delay(getTimeSpanBetween(3,4));
        var json = await api
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Post,
                "/lol-matchmaking/v1/ready-check/accept",
                Enumerable.Empty<string>());

    }

    public async Task<byte[]> manualRequest(HttpMethod Method, String uri)
    {
        HttpRequestMessage req = new HttpRequestMessage();
        var tokenVerde = "riot:" + api.RequestHandler.Token;
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenVerde));

        req.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {token}");
        req.RequestUri = new Uri($"https://127.0.0.1:{api.RequestHandler.Port}{uri}");
        req.Method = Method;
        
        var res = await client.SendAsync(req);

        return await res.Content.ReadAsByteArrayAsync();
    }
    
    public async Task<List<Champ>> getAllChamps(){
        if( api is null ) return new List<Champ>();
        
        var champsJsonData = await api.RequestHandler
            .GetResponseAsync<List<ChampsJSON>>(HttpMethod.Get,
                $"lol-champions/v1/inventories/{SummonerId}/champions");

        List<Task<byte[]>> task = 
            champsJsonData
                .Select(champ => manualRequest(HttpMethod.Get, champ.squarePortraitPath))
                .ToList();

        await Task.WhenAll(task);

        return champsJsonData
                .Select((x, i) => new Champ(x, task[i].Result))
                .ToList();
    
    }

    public async Task firstRequest()
    {
        var data = await api.RequestHandler.GetResponseAsync<string>(HttpMethod.Get, "/lol-gameflow/v1/gameflow-phase");
        await OnGameFlowEvent(data);
    }
}