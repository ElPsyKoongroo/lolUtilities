using System.Diagnostics;

namespace LeagueUtilities;

public partial class League{
    private async void OnGameflowEvent(object? sender, LeagueEvent e)
    {
        if(api is null) return;

        var msg = e.Data.ToString();
        
        OnGameFlowEvent(msg);
    }

    private async Task OnGameFlowEvent(string msg)
    {
        Debug.WriteLine(msg);
        switch(msg)
        {
            case "None":
            {
                phase = PHASES.NONE;
                break;
            }
            case "Lobby": //Lobby
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
                PickBan.New(api, SummonerId, hasToPick, hasToPickSkin, hasToInstaPick);
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
            case "WaitingForStats":
            {
                phase = PHASES.WAITINGFORSTATS;
                break;
            }
            case "EndOfGame":
            {
                phase = PHASES.ENDOFGAME;
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
