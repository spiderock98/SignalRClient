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
            const string connectionUrl = "http://localhost:5002/signalrhub";
            var hubClient = new SignalrClientHub(connectionUrl);

            // Register the "ReceiveMsg" event handler
            hubClient.RegisterEvent<IEnumerable<DataModel>>("ReceiveMsg", OnRecv);

            await hubClient.StartAsync();
            
            Console.ReadLine();
        }

        private static void OnRecv(IEnumerable<DataModel> items)
        {
            // Handle received data
            Console.WriteLine(JsonSerializer.Serialize(items));
        }
    }
}