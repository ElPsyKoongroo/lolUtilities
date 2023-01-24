using LiteDB;

namespace LeagueUtilities.JSON_Classes;
using System.Collections.Generic;
using LeagueUtilities.DTO;

public class PBProfile
{
    [BsonId] public ObjectId PBProfileId { get; set; }
    public string Name { get; set; }
    public List<ChampsJSON> picks { get; set; }
    public List<ChampsJSON> bans { get; set; }

    public PBProfile(){}

    public PBProfile(string Name, List<ChampsJSON> picks, List<ChampsJSON> bans)
    {
        this.PBProfileId = new ObjectId();
        this.Name = Name;
        this.picks = picks;
        this.bans = bans;
    }
}