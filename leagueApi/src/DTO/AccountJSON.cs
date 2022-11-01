
namespace LeagueUtilities.DTO
{
    public class SummonerJSON
    {
        public long AccountId { get; set; }
        public string DisplayName { get; set; }
        public string InternalName { get; set; }
        public bool NameChangeFlag { get; set; }
        public long PercentCompleteForNextLevel { get; set; }
        public string Privacy { get; set; }
        public long ProfileIconId { get; set; }
        public string Puuid { get; set; }
        public RerollPoints RerollPoints { get; set; }
        public long SummonerId { get; set; }
        public long SummonerLevel { get; set; }
        public bool Unnamed { get; set; }
        public long XpSinceLastLevel { get; set; }
        public long XpUntilNextLevel { get; set; }
    }

    public class RerollPoints
    {
        public long CurrentPoints { get; set; }
        public long MaxRolls { get; set; }
        public long NumberOfRolls { get; set; }
        public long PointsCostToRoll { get; set; }
        public long PointsToReroll { get; set; }
    }


}