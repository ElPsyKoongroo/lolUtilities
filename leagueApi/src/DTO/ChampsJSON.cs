namespace LeagueUtilities.DTO;
#nullable disable
public class ChampsJSON
{
    public int id { get; set; }
    public string alias { get; set; }
    public string name { get; set; }
    public string squarePortraitPath { get; set; }
    public List<string> roles { get; set; }
}