﻿using LeagueUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LeagueAPI;

public static class Program
{
    
    public static async Task Main()
    {
        //await A();
        //await Prueba2();
        Get_Sessions s = new();
        await s.GetSessions();
        Console.ReadKey();
    }



    /*   private static async Task Prueba3()
    {
        LeagueClientApi a = await LeagueClientApi.ConnectAsync();
    }
*/
    /*private static async Task Prueba1()
    {
        League n = new();
        n.addBans( 35,105,11,45);
        n.addPick(31, 517,61,555);

        Console.WriteLine("Conectando");
        await n.connect();
        Console.WriteLine("Conectado");

        Console.ReadKey();

        //await n.getSelectChampion();

        n.disconnect();

    }*/
    
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

    /*private static async Task Pruebas()
    {
        var lol = (await LeagueClientApi.ConnectAsync())!;

        const ulong idA = 102309419;
        var data = await lol
            .RequestHandler
            .GetResponseAsync<List<Champ>>(HttpMethod.Get, 
                $"/lol-champions/v1/inventories/{idA}/champions"
            );
        //data.RemoveAt(0);

        //data = data.Where(x => x.ownership.owned).ToList();

        var x = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

        await File.WriteAllTextAsync(@"C:\Users\Adrian\Desktop\lolChampsMinimal.txt", x);
    }*/
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
