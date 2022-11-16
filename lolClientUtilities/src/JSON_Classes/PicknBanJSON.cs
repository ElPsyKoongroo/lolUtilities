using System.Collections.Generic;
using LeagueUtilities.DTO;

namespace lolClientUtilities.JSON_Classes;

public class PicknBanJSON
{
    public List<ChampsJSON> picks { get; set; }
    public List<ChampsJSON> bans { get; set; }
}