using System.Text.Json.Serialization;
using LeagueUtilities;
using LCUSharp;
using LCUSharp.Websocket;

namespace LeagueAPI;

public static class Program
{
    public static async Task Main()
    {
        await Pruebas();
    }

    private static async Task Prueba1()
    {
        League n = new();
        n.addBans(114,105,67);
        n.addPick(238,35,134);

        await n.connect();

        Console.ReadKey();

        await n.getSelectChampion();

        n.disconnect();

    }

    private static async Task Pruebas()
    {
        var lol = (await LeagueClientApi.ConnectAsync())!;

        ulong idA = 102309419;
        var data = await lol
            .RequestHandler
            .GetResponseAsync<List<Champ>>(HttpMethod.Get, 
                $"/lol-champions/v1/inventories/{idA}/champions"
            );
        data.RemoveAt(0);

        data = data.Where(x => x.ownership.owned).ToList();

        Console.WriteLine(data.Count);
    }
}
public class Champ
{
    public string alias;
    public string name;
    public long id;
    public Ownership ownership;

    public class Ownership
    {
        public bool owned;
    }
}

