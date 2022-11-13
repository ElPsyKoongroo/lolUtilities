using System.Diagnostics;

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
                Debug.WriteLine(hasToAutoAccept);
                break;
            }
            case "ReadyCheck":
            {
                if (!hasToAutoAccept) return;
                await acceptGame();
                break;
            }
            case "MatchMaking":
            {
                PickBan.Finish();
                break;
            }
            case "ChampSelect":
            {
                phase = PHASES.CHAMPSELECT;
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                PickBan.New(api, SummonerId, hasToPick, hasToPickSkin);
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