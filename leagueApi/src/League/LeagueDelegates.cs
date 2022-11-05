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

        var msg = e.Data.ToString();

        switch(msg)
        {
            case "Lobby":
            {
                PickBan.Finish();

                break;
            }
            case "ReadyCheck":
            {
                await Task.Delay(getTimeSpanBetween(1,2));
                await api
                    .RequestHandler
                    .GetJsonResponseAsync(HttpMethod.Post,
                        "/lol-matchmaking/v1/ready-check/accept",
                        Enumerable.Empty<string>());
                break;
            }
            case "MatchMaking":
            {
                PickBan.Finish();
                break;
            }
            case "ChampSelect":
            {   
                PickBan.New(api, SummonerId, true, true);
                PickBan.SetPicks(champsToBanId,champsToPickId);
                await PickBan.Start();
                break;
            }
            case "InProgress":
            {
                PickBan.Finish();
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