using System.Diagnostics;

namespace LeagueUtilities;

public partial class League{
    private async void OnGameflowEvent(object? sender, LeagueEvent e)
    {
        if(api is null) return;

        var msg = e.Data.ToString();

        switch(msg)
        {
            case "None":
            {
                phase = PHASES.NONE;
                break;
            }
            case "Lobby":
            {
                phase = PHASES.LOBBY;
                PickBan.Finish();
                break;
            }
            case "MatchMaking":
            {
                phase = PHASES.MATCHMAKING;
                PickBan.Finish();
                break;
            }
            case "ReadyCheck":
            {
                phase = PHASES.READYCHECK;
                if (!hasToAutoAccept) return;
                await acceptGame();
                break;
            }
            case "ChampSelect":
            {
                phase = PHASES.CHAMPSELECT;
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                PickBan.New(api, SummonerId, hasToPick, hasToPickSkin);
                PickBan.SetPicks(champsToBanId,champsToPickId, orderToPick);
                
                await PickBan.Start();
                
                break;
            }
            case "InProgress":
            {
                phase = PHASES.INPROGRESS;
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