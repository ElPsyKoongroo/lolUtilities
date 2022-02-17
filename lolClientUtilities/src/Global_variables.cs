using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lolClientUtilities.src
{
    public class Global_variables
    {
        public static Dictionary<string, string> GetPaths = new()
        {
            { "ChampSelectInfo", "/lol-champ-select/v1/session" },
            { "ClientStatus", "/lol-gameflow/v1/gameflow-phase" },
            { "SummonerAccountID", "/lol-summoner/v1/current-summoner/account-and-summoner-ids"},
            { "AllChampsReduced", "/lol-champions/v1/inventories/{id}/champions-minimal" },
            { "AllChamps", "/lol-champions/v1/inventories/{id}/champions" },
        };
        public static Dictionary<string, string> PostPaths = new()
        {
            { "AcceptGame", "/lol-matchmaking/v1/ready-check/accept" }
        };

        public static Dictionary<string, string> Paths = new()
        {
            { "" , "" },
        };
    }
}
