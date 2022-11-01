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
            n.addBans(35,45,105);
            n.addPick(517,555,53);

            await n.connect();

            Console.ReadKey();

            n.disconnect();

        }

        public static void Log(string log)
        {
            string hora = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{hora}]  {log}");
        }
    }


}
