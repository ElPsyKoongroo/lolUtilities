using Serilog;
using LCUSharp.Websocket;
using LeagueUtilities.DTO;

namespace LeagueUtilities;

public partial class League{
/*
  ReadyCheck -> Aceptar partida
  None -> Entrar al lol. Sin estar en el lobby.
  Lobby -> Estás en lobby.
  Matchmaking -> En queue.
  ChampSelect -> self explanatory.
  InProgress -> entrando en partida/ en partida.
  WaitingForStats -> stats.
  EndOfGame -> stats
*/
    private async void OnGameflowEvent(object? sender, LeagueEvent e)
    {
        if(api is null) return;

        string msg = e.Data.ToString();

        switch(msg)
        {
            case "Lobby":
            {
                pickBan?.Finish();

                break;
            }
            case "ReadyCheck":
            {
                await Task.Delay(getTimeSpanBetween(1,2));
                var json = await api
                        .RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Post,
                        "/lol-matchmaking/v1/ready-check/accept",
                        Enumerable.Empty<string>());
                break;
            }
            case "MatchMaking":
            {
                pickBan?.Finish();
                break;
            }
            case "ChampSelect":
            {   
                pickBan = new(api, SummonerId, true, true);
                pickBan.SetPicks(champsToBanId,champsToPickId);
                break;
            }
            case "InProgress":
            {
                pickBan?.Finish();
                break;
            }
            
        }
    }

    
    private async void OnDisconnected(object? sender, EventArgs e)
    {
        if (api is null) return;
        await api.ReconnectAsync();
    }

}