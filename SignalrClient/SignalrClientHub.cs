using Microsoft.AspNetCore.SignalR.Client;

namespace SignalrClient;

public class SignalrClientHub
{
    private readonly HubConnection _connection;

    public SignalrClientHub(string connectionUrl)
    {
        _connection = InitHubConnection(connectionUrl);
    }

    private HubConnection InitHubConnection(string conn)
    {
        // Configure and build the HubConnection
        var connection = new HubConnectionBuilder()
            .WithUrl(conn)
            .WithAutomaticReconnect(Enumerable.Repeat(TimeSpan.FromSeconds(1), (int)TimeSpan.FromMinutes(30).TotalSeconds).ToArray())
            .Build();

        return connection;
    }

    public void RegisterEvent<T>(string eventName, Action<T> callback)
    {
        _connection.On<T>(eventName, items => callback(items));
    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            Console.WriteLine("Connection started successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting connection: {ex.Message}");
        }
    }
}