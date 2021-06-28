using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TIN_Client.Services;

namespace TIN_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ChatService service;

        public string userName;

        protected override void OnStartup(StartupEventArgs e)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chathub")
                .Build();
            connection.StartAsync();

            service = new ChatService(connection);

            userName = string.Empty;

            base.OnStartup(e);
        }
    }
}
