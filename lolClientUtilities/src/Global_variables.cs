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
