using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SignalrClient;

namespace SignalRClient
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Program started ....");

            // Initialize HubConnection
            const string connectionUrl = "http://localhost:5093/signalrhub";
            // const string connectionUrl = "http://localhost:5002/systemnotificationhub";

            // const string connectionUrl = "https://takako.viotapp.cloud/signalrhub";
            var hubClient = new SignalrClientHub(connectionUrl);

            // Register the "ReceiveMsg" event handler
            // hubClient.Register<IEnumerable<GsmModel>>("ReceiveMsg", OnRecv);
            hubClient.Register_v2<object>("ReceiveMsg", s =>
            {
                Console.WriteLine(JsonSerializer.Serialize(s));
                return new { Message = "I see you", Code = 200 };
            });

            // Add event callback
            hubClient.Connection.Closed += _ =>
            {
                // Do something
                return Task.CompletedTask;
            };
            hubClient.Connection.Reconnecting += _ =>
            {
                // Do something

                return Task.CompletedTask;
            };
            hubClient.Connection.Reconnected += _ =>
            {
                // Do something

                return Task.CompletedTask;
            };
            hubClient.Connected += (_, _) =>
            {
                // Do something
            };

            // start signalr server connection
            await hubClient.StartConnectionAsync();

            // send test message to server
            Console.WriteLine("Sending new message");
            await hubClient.SendMessage("OnMessageArrive", "haha");


            // Stop program here
            Console.ReadLine();
        }

        private static void OnRecv(IEnumerable<GsmModel> items)
        {
            // Handle received data
            Console.WriteLine(JsonSerializer.Serialize(items));
        }
    }
}