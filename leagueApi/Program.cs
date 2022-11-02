using LCUSharp.Websocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using LCUSharp;
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

