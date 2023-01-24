using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueUtilities.DTO;
using LiteDB;

namespace lolClientUtilities.JSON_Classes;

public class PicknBanJSON
{
    public List<PicknBanProfile> Profiles { get; set; }

    public PicknBanJSON(IEnumerable<PicknBanProfile> Profiles)
    {
        this.Profiles = Profiles.ToList();
    }
}
public class PicknBanProfile
{
    [BsonId] public ObjectId id { get; set; }
    public string Nombre { get; set; }
    public List<ChampsJSON> picks { get; set; }
    public List<ChampsJSON> bans { get; set; }

    public PicknBanProfile(string Nombre, List<ChampsJSON> picks, List<ChampsJSON> bans)
    {
        this.Nombre = Nombre;
        this.picks = picks;
        this.bans = bans;
    }
}