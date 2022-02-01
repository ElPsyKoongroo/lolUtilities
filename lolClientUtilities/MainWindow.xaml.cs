using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Text.Json;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using WebSocketSharp;
using System.Threading;

namespace lolClientUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string searchLol = "/C wmic PROCESS WHERE name='LeagueClientUx.exe' GET commandline";
        public HttpClient client;
        public WebSocketSharp.WebSocket sock;
        string url, url2, req, req2, token, token2;
        string req_perks = "/lol-perks/v1/pages";
        ClientWebSocket sock2 = new();

        public MainWindow()
        {
            InitializeComponent();

            /*var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => {return true;};

            client = new HttpClient(handler);*/
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            string Output;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = "cmd.exe";
            psi.Arguments = searchLol;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
            Output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Match portMatch = Regex.Match(Output, @"--app-port=([0-9]+)");
            Match tokenMatch = Regex.Match(Output, @"--remoting-auth-token=([\w-]+)");

            if (!portMatch.Success || !tokenMatch.Success)
            {
                txtOutput.Text = "Lol no abierto";
                return;
            }

            int port = int.Parse(portMatch.Groups[1].Value);
            string auxString = tokenMatch.Groups[1].Value;

            byte[] tokenByteArray = Encoding.ASCII.GetBytes($"riot:{auxString}");

            token = Convert.ToBase64String(tokenByteArray);
            token2 = auxString;
            url = $"https://127.0.0.1:{port}";
            url2 = $"wss://127.0.0.1:{port}";
            req = $"{url}/lol-perks/v1/pages";
            req2 = $"{url2}/lol-perks/v1/pages";

            Trace.WriteLine($"URL: {url}\nREQ: {req}\nToken: {token}");
            Trace.WriteLine("\n\n");

        }
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, req);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
            HttpResponseMessage response;

            try
            {
                response = client.Send(request);
                string Out = response.Content.ReadAsStringAsync().Result;

                var x = JsonSerializer.Deserialize<JsonElement>(Out);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                Out = JsonSerializer.Serialize(x, options);

                Trace.WriteLine(Out);

                //System.IO.File.WriteAllText("hey.txt", Out);


            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error");
            }
        }
        private async void btn3_Click(object sender, RoutedEventArgs e)
        {
            /*
            sock = new ($"{url2}/");
            sock.SslConfiguration.ServerCertificateValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };

            //sock.SetCredentials("riot", token2, true);


            sock.OnMessage += mensajeRecibido;
            sock.OnOpen += Sock_OnOpen;
            sock.OnError += Sock_OnError;
            sock.OnClose += Sock_OnClose;

            Trace.WriteLine("Connect");
            sock.Connect();
            Trace.WriteLine("Accept");
            */

            
            sock2.Options.SetRequestHeader("Authorization", $"Basic {token}");
            var cts = new CancellationTokenSource();

            sock2.Options.RemoteCertificateValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };

            

            await sock2.ConnectAsync(new Uri(url2), cts.Token);

            Trace.WriteLine(sock2.State);

            var y = new ArraySegment<byte>(new byte[1024]);
            ArraySegment<byte> array = new();
            array = Encoding.ASCII.GetBytes("[5, \"OnJsonApiEvent\"]");

            await sock2.SendAsync(array, WebSocketMessageType.Text, false, cts.Token);

            Trace.WriteLine(sock2.State);


            Trace.WriteLine("Iniciando request");

            while (true)
            {
                var x = await sock2.ReceiveAsync(array, cts.Token);
                Trace.WriteLine("Terminado request");

                Trace.WriteLine(array.ToString());

            }
        }
        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            sock.Close();
        }
        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            sock.Send(req_perks);
        }
        private void btn6_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Sock_OnClose(object? sender, CloseEventArgs e)
        {
            Trace.WriteLine($"Cerrado: {e.Reason}\n{e.Code}");
        }
        private void Sock_OnError(object? sender, ErrorEventArgs e)
        {
            Trace.WriteLine("Error: " + e.Message);
        }
        private void Sock_OnOpen(object? sender, EventArgs e)
        {
            Trace.WriteLine("WSS open");
        }
        private void mensajeRecibido(object? sen, MessageEventArgs e)
        {
            Trace.WriteLine("Mensaje: "+ e.Data.ToString());
        }

    }
}
