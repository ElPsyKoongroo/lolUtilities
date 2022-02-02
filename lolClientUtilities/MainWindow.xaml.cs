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
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.ComponentModel;

using LCUSharp;
using LCUSharp.Websocket;
using lolClientUtilities.src;


/*
 TODO

    Comprobar al desconectarte si es a causa de que el lol se ha cerrado o que se ha entrado en partida // lol-shutdown/v1/notification
 
 
 */

namespace lolClientUtilities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        LeagueClientApi cliente;
        bool isConnectedToClient = false;

        private string gameStatus = "None";

        public string GameStatus
        {
            get { return gameStatus; }
            set
            {
                gameStatus = value;
                OnPropertyChange("GameStatus");
            }
        }

        #region Eventos

        private event EventHandler<LeagueEvent> GameFlowEvent;

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion


        #region Events Handlers
        public void OnPropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void OnGameFlowChange(object? sender, LeagueEvent e)
        {
            string value = e.Data.ToString();
            Trace.WriteLine("GameFlow change");
            GameStatus = value;

            if(checkBoxAutoAccept.IsChecked.Value && value == "ReadyCheck")
            {
                var json =
                    await cliente
                        .RequestHandler
                        .GetJsonResponseAsync(HttpMethod.Post,
                         "/lol-matchmaking/v1/ready-check/accept",
                         Enumerable.Empty<string>());
            }
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();
            InitiateEvents();

            InitBindings();
        }

        private void InitiateEvents()
        {
            GameFlowEvent += OnGameFlowChange;
        }

        private void InitClientEvents()
        {
            cliente.Disconnected += Cliente_Disconnected;
        }

        private void InitBindings()
        {
            this.DataContext = this;
        }

        private async void Cliente_Disconnected(object? sender, EventArgs e)
        {
            isConnectedToClient=false;

            Trace.WriteLine("Desconectado, volviendo a conectar...");

            await cliente.ReconnectAsync();

            Trace.WriteLine("Conectado de nuevo");

            isConnectedToClient = true;
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (isConnectedToClient) return;

            Trace.WriteLine("Conectando");
            cliente = await LeagueClientApi.ConnectAsync();
            Trace.WriteLine("Conectado");

            InitClientEvents();

            cliente.EventHandler.Subscribe(Global_variables.GetPaths["ClientStatus"], GameFlowEvent);

            isConnectedToClient = true;
        }

        
        
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btn3_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btn6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cliente?.Disconnect();
        }
    }
}
