using LeagueUtilities;

namespace LeagueAPI;

public class Program
{
    public static async Task Main()
    {
        
        League n = new();
        n.addBans(114,105,67);
        n.addPick(238,35,134);

        await n.connect();

        Console.ReadKey();

        n.disconnect();

    }
}

