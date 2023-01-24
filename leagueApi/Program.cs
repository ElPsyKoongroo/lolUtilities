using System.Net;
using System.Net.Http.Headers;
using System.Text;
using LeagueUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LeagueAPI;

public static class Program
{
    
    public static async Task Main()
    {
        /*Get_Sessions s = new();
        await s.GetSessions();*/

        //await RequestManual();

        await CalcularEsenciasBotin();
    }

    private static async Task CalcularEsenciasBotin()
    {
        LeagueClientApi api = await LeagueClientApi.ConnectAsync();

        var response =
            await
                api.RequestHandler.GetResponseAsync<List<dynamic>>(HttpMethod.Get, "/lol-loot/v1/player-loot");

        Console.WriteLine("Campeones: ");
        var total = 0;
        foreach (var loot in response)
        {
            if (loot.type == "CHAMPION_RENTAL")
            {
                total += (int)loot.disenchantValue * (int)loot.count;
                Console.WriteLine(loot.itemDesc + ": " + (int)loot.disenchantValue * (int)loot.count);
            }
        }

        Console.WriteLine("Total esencias azules: " + total);
        total = 0;
        Console.WriteLine("\n\nSkins: ");
        foreach (var loot in response)
        {
            if (loot.type == "SKIN_RENTAL")
            {
                total += (int)loot.disenchantValue * (int)loot.count;
                Console.WriteLine(loot.itemDesc + ": " + (int)loot.disenchantValue * (int)loot.count);
            }
        }

        Console.WriteLine("Total esencias naranjas: " + total);
    }

    private static async Task RequestManual()
    {
        LeagueClientApi api = await LeagueClientApi.ConnectAsync();


        HttpClientHandler handler = new HttpClientHandler();

        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            return true;
        };

        HttpClient client = new(handler);

        HttpRequestMessage req = new HttpRequestMessage();
        var tokenVerde = "riot:" + api.RequestHandler.Token;
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenVerde));


        req.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {token}");
        req.RequestUri =
            new Uri($"https://127.0.0.1:{api.RequestHandler.Port}/lol-game-data/assets/v1/champion-icons/1.png");
        req.Method = HttpMethod.Get;


        var res = await client.SendAsync(req);

        var data = await res.Content.ReadAsByteArrayAsync();


        await File.WriteAllBytesAsync(@"C:\Users\Adrian\Desktop\LOL\data.jpg", data);

        Console.WriteLine("Terminado");
        //Console.ReadKey();
    }

    private static async Task Prueba2()
    {
        //n.addBans( 35,105,11,45);
        //n.addPick(31, 517,61,555);

        var actualPath = Environment.CurrentDirectory;

        var champs = Path.Combine(actualPath, "campeones.txt");
        if (!File.Exists(champs))
        {
            Console.WriteLine("Hace falta tener el archivo 'campeones.txt'");
            Console.ReadKey();
            return;
        }

        var info = await File.ReadAllTextAsync(champs);

        ChampsAux champsData;
        try
        {
            champsData = JsonSerializer.Deserialize<ChampsAux>(info, new JsonSerializerOptions { IncludeFields = true })!;
        }
        catch (Exception e)
        {
            Console.WriteLine("El archivo 'campeones.txt' no esta bien escrito");
            Console.WriteLine(e.Message);
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Campeones para banear: ");
        foreach (var champ in champsData.bans)
        {
            Console.WriteLine(champ);
        }

        Console.WriteLine("\n\n");
        
        Console.WriteLine("Campeones para pickear: ");
        foreach (var champ in champsData.picks)
        {
            Console.WriteLine(champ);
        }

        League n = League.GetLeague();
        Console.WriteLine("Conectando");
        await n.connect();
        Console.WriteLine("Conectado");
        
        n.addBans( champsData.bans.ToArray());
        n.addPick(champsData.picks.ToArray());

        Console.ReadKey();


        n.disconnect();

    }
}
public class ChampsAux
{
    public List<int> bans { get; set; }
    public List<int> picks { get; set; }
}

public class Get_Sessions
{
    private LeagueClientApi api;
    private int cont = 0;
    private string path = Path.Combine(Environment.CurrentDirectory, "sessions", "session_"); 
    public async Task GetSessions()
    {
        api = await LeagueClientApi.ConnectAsync();
         
        api?.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", OnGameflowEvent);

        Console.WriteLine("Esperando");
    }
    
    
    private async void OnGameflowEvent(object? sender, LeagueEvent e)
    {
        if (e.Data.ToString() != "ChampSelect") return;
        
        var data = await api
            .RequestHandler
            .GetJsonResponseAsync(
                HttpMethod.Get,
                "/lol-champ-select/v1/session"
            );
        api.EventHandler.Subscribe("/lol-champ-select/v1/session", OnSessionEvent);

        Console.WriteLine("Ha entrado");
        
        await Write(data);
    }

    private async void OnSessionEvent(object? sender, LeagueEvent e)
    {
        var sessionData = e.Data.ToString();
        if (sessionData is null) return;
        
        await Write(sessionData);
    }

    private async Task Write(string data)
    {
        Console.WriteLine($"Escribiendo {++cont}");

        data = JToken.Parse(data).ToString(Formatting.Indented);

        data = $"{DateTime.Now:hh:mm:ss.ffffff}:\n\n{data}";

        var file = File.CreateText($"{path}{cont}");

        await file.WriteAsync(data);
        
        file.Close();
    }
}
