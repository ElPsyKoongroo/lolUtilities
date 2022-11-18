namespace LeagueUtilities.DTO;
#nullable disable
public class SkinJSON
{
    public int championId { get; set; }
    public string chromaPath { get; set; }
    public Chroma[] chromas { get; set; }
    public object collectionSplashVideoPath { get; set; }
    public bool disabled { get; set; }
    public object[] emblems { get; set; }
    public object featuresText { get; set; }
    public int id { get; set; }
    public bool isBase { get; set; }
    public bool lastSelected { get; set; }
    public string loadScreenPath { get; set; }
    public string name { get; set; }
    public Ownership ownership { get; set; }
    public Questskininfo questSkinInfo { get; set; }
    public string rarityGemPath { get; set; }
    public string skinType { get; set; }
    public string splashPath { get; set; }
    public object splashVideoPath { get; set; }
    public bool stillObtainable { get; set; }
    public string tilePath { get; set; }
    public string uncenteredSplashPath { get; set; }
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

public class Questskininfo
{
    public string collectionCardPath { get; set; }
    public string collectionDescription { get; set; }
    public object[] descriptionInfo { get; set; }
    public string name { get; set; }
    public string splashPath { get; set; }
    public object[] tiers { get; set; }
    public string tilePath { get; set; }
    public string uncenteredSplashPath { get; set; }
}

public class Chroma
{
    public int championId { get; set; }
    public string chromaPath { get; set; }
    public string[] colors { get; set; }
    public bool disabled { get; set; }
    public int id { get; set; }
    public bool lastSelected { get; set; }
    public string name { get; set; }
    public Ownership ownership { get; set; }
    public bool stillObtainable { get; set; }
}
