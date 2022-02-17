using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lolClientUtilities.src.JSON_Classes
{
    public class SummonerChampInfoReducedJSON
    {
        public bool active { get; set; }
        public string alias { get; set; }
        public string banVoPath { get; set; }
        public string baseLoadScreenPath { get; set; }
        public string baseSplashPath { get; set; }
        public bool botEnabled { get; set; }
        public string chooseVoPath { get; set; }
        public object[] disabledQueues { get; set; }
        public bool freeToPlay { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public Ownership ownership { get; set; }
        public long purchased { get; set; }
        public bool rankedPlayEnabled { get; set; }
        public string[] roles { get; set; }
        public string squarePortraitPath { get; set; }
        public string stingerSfxPath { get; set; }
        public string title { get; set; }
    }

    public class Ownership
    {
        public bool freeToPlayReward { get; set; }
        public bool owned { get; set; }
        public Rental rental { get; set; }
    }

    public class Rental
    {
        public int endDate { get; set; }
        public long purchaseDate { get; set; }
        public bool rented { get; set; }
        public int winCountRemaining { get; set; }
    }

}
