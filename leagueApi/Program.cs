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

namespace LeagueAPI
{
    public class Program
    {
        public async static Task Main()
        {
            League n = new();
            n.addBans(22,1,2);

            await n.connect();
        }

        public static void Log(string log)
        {
            string hora = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{hora}]  {log}");
        }
    }


}
