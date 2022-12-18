using System.Threading.Channels;
using LeagueUtilities.DTO;

namespace LeagueUtilities.Models;
#nullable disable

public class Champ
{
    public int id { get; set; }
    public string alias { get; set; }
    public string name { get; set; }
    public string squarePortraitPath { get; set; }
    public List<string> roles { get; set; }
    public byte[] image { get; set; }

    public Champ(ChampsJSON champ, byte[] img)
    {
        id = champ.id;
        alias = champ.alias;
        name = champ.name;
        image = img;
        roles = champ.roles;
    }

    public ChampsJSON AsChampJSON()
    {
        return new ChampsJSON()
        {
            alias = alias,
            name = name,
            id = id,
            roles = roles,
            squarePortraitPath = squarePortraitPath
        };
    }
}