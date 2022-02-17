using LCUSharp.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using LCUSharp;

/*
     # Auto accept match
    if phase == 'ReadyCheck':
        r = request('post', '/lol-matchmaking/v1/ready-check/accept')  
            # '/lol-lobby-team-builder/v1/ready-check/accept')
 
champions = {"1":"Annie","2":"Olaf","3":"Galio","4":"Twisted Fate",
"5":"Xin Zhao","6":"Urgot","7":"LeBlanc","8":"Vladimir","9":"Fiddlesticks",
"10":"Kayle","11":"Master Yi","12":"Alistar","13":"Ryze","14":"Sion","15":"Sivir",
"16":"Soraka","17":"Teemo","18":"Tristana","19":"Warwick","20":"Nunu","21":"Miss Fortune",
"22":"Ashe","23":"Tryndamere","24":"Jax","25":"Morgana","26":"Zilean","27":"Singed",
"28":"Evelynn","29":"Twitch","30":"Karthus","31":"Cho\'Gath","32":"Amumu","33":"Rammus",
"34":"Anivia","35":"Shaco","36":"Dr. Mundo","37":"Sona","38":"Kassadin","39":"Irelia",
"40":"Janna","41":"Gangplank","42":"Corki","43":"Karma","44":"Taric","45":"Veigar",
"48":"Trundle","50":"Swain","51":"Caitlyn","53":"Blitzcrank","54":"Malphite","55":"Katarina",
"56":"Nocturne","57":"Maokai","58":"Renekton","59":"Jarvan IV","60":"Elise","61":"Orianna",
"62":"Wukong","63":"Brand","64":"Lee Sin","67":"Vayne","68":"Rumble","69":"Cassiopeia",
"72":"Skarner","74":"Heimerdinger","75":"Nasus","76":"Nidalee","77":"Udyr","78":"Poppy",
"79":"Gragas","80":"Pantheon","81":"Ezreal","82":"Mordekaiser","83":"Yorick","84":"Akali",
"85":"Kennen","86":"Garen","89":"Leona","90":"Malzahar","91":"Talon","92":"Riven",
"96":"Kog\'Maw","98":"Shen","99":"Lux","101":"Xerath","102":"Shyvana","103":"Ahri",
"104":"Graves","105":"Fizz","106":"Volibear","107":"Rengar","110":"Varus","111":"Nautilus",
"112":"Viktor","113":"Sejuani","114":"Fiora","115":"Ziggs","117":"Lulu","119":"Draven",
"120":"Hecarim","121":"Kha\'Zix","122":"Darius","126":"Jayce","127":"Lissandra","131":"Diana",
"133":"Quinn","134":"Syndra","136":"Aurelion Sol","141":"Kayn","142":"Zoe","143":"Zyra",
"145":"Kai\'Sa","150":"Gnar","154":"Zac","157":"Yasuo","161":"Vel\'Koz","163":"Taliyah",
"164":"Camille","201":"Braum","202":"Jhin","203":"Kindred","222":"Jinx","223":"Tahm Kench",
"236":"Lucian","238":"Zed","240":"Kled","245":"Ekko","254":"Vi","266":"Aatrox","267":"Nami",
"268":"Azir","412":"Thresh","420":"Illaoi","421":"Rek\'Sai","427":"Ivern","429":"Kalista",
"432":"Bard","497":"Rakan","498":"Xayah","516":"Ornn","555":"Pyke"} # ,"-1":"None"}

 
 */

namespace preubasConsola
{
    public class Program
    {
        public async static Task Main()
        {
            Prueba6 p = new();

            await p.test();
        }

        public static void Log(string log)
        {
            string hora = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{hora}]  {log}");
        }
    }


    #region Prueba1
    public class Prueba1
    {
        public async Task prueba()
        {
            // Initialize a connection to the league client.
            var api = await LeagueClientApi.ConnectAsync();


            await api.RiotClientEndpoint.FlashUxAsync();

            
            // Show the client.
            await api.RiotClientEndpoint.ShowUxAsync();
            await Task.Delay(1000);

            // Update the current summoner's profile icon to 23.
            var body = new { profileIconId = 23 };
            var queryParameters = Enumerable.Empty<string>();

            Stopwatch a = new();
            a.Start();

            var json = await api
                .RequestHandler
                .GetJsonResponseAsync(HttpMethod.Get,
                "lol-summoner/v1/current-summoner",
                queryParameters, body);

            dynamic data = JObject.Parse(json);

            dynamic data2 = JsonSerializer.Deserialize<dynamic>(data);

            a.Stop();
            //Console.WriteLine($"{a.ElapsedMilliseconds/100.0}s -> {data.profileIconId}");

            foreach (var x in data)
            {
                Console.WriteLine($"Info->{x}");
            }

            foreach (var x in data2)
            {
                Console.WriteLine($"Info->{x}");
            }

            // Minimize the client.
            await Task.Delay(1000);
            await api.RiotClientEndpoint.MinimizeUxAsync();
        }
    }

    #endregion

    #region Prueba2
    public class Prueba2
    {
        public event EventHandler<LeagueEvent> GameFlowChanged;
        //private readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);

        public async Task test()
        {
            // Initialize a connection to the league client.
            var api = await LeagueClientApi.ConnectAsync();
            Console.WriteLine("Connected!");

            // Register game flow event.
            GameFlowChanged += OnGameFlowChanged;
            api.EventHandler.Subscribe("/lol-gameflow/v1/gameflow-phase", GameFlowChanged);

            // Wait until work is complete.
            //await _work.Task;


            Console.Read();

            Console.WriteLine("Done.");
        }

        private void OnGameFlowChanged(object sender, LeagueEvent e)
        {
            var result = e.Data.ToString();
            var state = string.Empty;

            switch (result)
            {
                case "None":
                    state = "main menu";
                    break;
                case "Lobby":
                    state = "lobby";
                    break;
                case "ChampSelect":
                    state = "champ select";
                    break;
                case "GameStart":
                    state = "game started";
                    break;
                case "InProgress":
                    state = "game";
                    break;
                case "WaitingForStats":
                    state = "waiting for stats";
                    break;
                default:
                    state = $"unknown state: {result}";
                    break;
            }

            // Print new state and set work to complete.
            Console.WriteLine($"Status update: Entered {state}.");
            //_work.SetResult(true);
        }
    }
    #endregion

    
    #region Prueba3 Paginas de runas

    public class Prueba3
    {
        public event EventHandler<LeagueEvent> GameFlowChanged;
        private string lastPerksPage = String.Empty;

        public async Task test()
        {
            // Initialize a connection to the league client.
            var api = await LeagueClientApi.ConnectAsync();
            Program.Log("Connected!");

            // Register game flow event.
            GameFlowChanged += OnGameFlowChanged;
            api.EventHandler.Subscribe("/lol-perks/v1/pages", GameFlowChanged);

            Console.Read();

            Program.Log("Done.");
        }

        private void OnGameFlowChanged(object sender, LeagueEvent e)
        {
            /*dynamic data = JObject.Parse(e.Data.ToString());

            foreach(var x in data)
            {
                if(x.name == "ditto")
                {
                    Console.WriteLine(x.lastModified);
                }
            }*/

            if (lastPerksPage == String.Empty) { lastPerksPage = e.Data.ToString(); }

            else if (lastPerksPage == e.Data.ToString()) return;

            Program.Log("Se han cambiado las runas");
            lastPerksPage = e.Data.ToString();

            return;
        }
    }

    #endregion

    #region Prueba 4 Campeon actual

    class Prueba4
    {
        private event EventHandler<LeagueEvent> eventoSession;
        private event EventHandler<LeagueEvent> gameflow;

        int cellid = -1;

        int actual = 0;

        src.Action? lastAction;


        public async Task test()
        {
            var api = await LeagueClientApi.ConnectAsync();

            Program.Log("Iniciado");

            eventoSession += OnChangeSession;

            gameflow += Prueba4_gameflow;

            api .EventHandler
               .Subscribe("/lol-champ-select/v1/session", eventoSession);

            api.EventHandler
                .Subscribe("/lol-gameflow/v1/gameflow-phase", gameflow);

            Console.Read();

            Program.Log("Terminando");
        }

        private void Prueba4_gameflow(object? sender, LeagueEvent e)
        {
            string flow = e.Data.ToString();
            Program.Log(flow);

            if(flow == "ChampSelect")
            {
                cellid = -2;
                return;
            }
            cellid = -1;
            if(flow == "None")
            {
                Console.Clear();
                return;
            }
        }

        private void OnChangeSession(object sender, LeagueEvent e)
        {
            src.SessionsJSON sessionsJson = JsonSerializer.Deserialize<src.SessionsJSON>(e.Data.ToString(), new JsonSerializerOptions { IncludeFields = true });

            if (sessionsJson.localPlayerCellId == -1) return;
            if (cellid == -2)
            {
                cellid = sessionsJson.localPlayerCellId;

                Program.Log($"CellID-> {cellid}");
            }

            foreach(var action in sessionsJson.actions[^1])
            {
                if(action.actorCellId == cellid)
                {
                    if (lastAction != null && action.id == lastAction.id) return;
                    lastAction = action;
                    //File.WriteAllText($"pipo{actual++}.txt", JsonSerializer.Serialize(sessionsJson, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true }));
                    if (action.type == "ban") Program.Log("Te toca banear crack");
                    else if (action.type == "pick") Program.Log("Te toca pickear crack");
                }
            }
            
        }

    }

    #endregion

    #region Prueba 5 Auto Aceptar

    public class Prueba5
    {
        private event EventHandler<LeagueEvent> evento;
        LeagueClientApi api;
        public async Task test()
        {
            api = await LeagueClientApi.ConnectAsync();

            Program.Log("Iniciado");

            evento += OnChange;
            api.Disconnected += Api_Disconnected;

            api.EventHandler
                .Subscribe("/lol-gameflow/v1/gameflow-phase", evento);

            Console.Read();

            Program.Log("Terminando");
        }

        private async void Api_Disconnected(object sender, EventArgs e)
        {
            Program.Log("Desconectado");
            await api.ReconnectAsync();
            Program.Log("Conectado");
        }

        private async void OnChange(object sender, LeagueEvent e)
        {
            if (e.Data.ToString() == "ReadyCheck")
            {
                Program.Log("Aceptando partida");
                var json =
                    await api
                        .RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Post,
                         "/lol-matchmaking/v1/ready-check/accept",
                         Enumerable.Empty<string>());
            }
        }
    }

    #endregion

    #region Prueba 6 GetChamps

    public class Prueba6
    {
        public LeagueClientApi api;

        public async Task test()
        {
            api = await LeagueClientApi.ConnectAsync();

            long id;

            Program.Log("Conectado Prueba 6");
            ///lol-summoner/v1/current-summoner/account-and-summoner-ids;


            var jsonID = await api.RequestHandler.GetJsonResponseAsync(HttpMethod.Get,
                $"/lol-summoner/v1/current-summoner/account-and-summoner-ids");

            id = (JObject.Parse(jsonID.ToString()) as dynamic).summonerId;

            Console.WriteLine(id);

            var json = await api.RequestHandler.GetJsonResponseAsync(HttpMethod.Get,
                $"/lol-champions/v1/inventories/{id}/champions-minimal");

            if(json == null)
            {
                Console.WriteLine("Nulo");
                return;
            }
            //Console.WriteLine(json);

            var js = JsonSerializer.Deserialize<List<object>>(json, new JsonSerializerOptions { IncludeFields = true });



            foreach(var cham in js)
            {
                dynamic champ = JObject.Parse(cham.ToString());

                //Console.WriteLine(champ);
                Console.WriteLine($"Owned->\t {champ.ownership.owned}");
                Console.WriteLine($"ID  ->\t {champ.id}");
                Console.WriteLine($"Name->\t {champ.name}\n\n");
            }

            Console.WriteLine($"\n\nTotal-> {js.Count}");


            Console.Read();
        }
    }

    #endregion

}
