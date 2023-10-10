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
            .Build();

        connection.Closed += async exception =>
        {
            Console.WriteLine(exception != default
                ? $"[ERR] Connection closed: {exception.Message}"
                : "[INFO] Connection closed without error.");

            while (true)
            {
                if (await StartAsync()) break;
                await Task.Delay((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
            }
        };

        return connection;
    }

    public void Register<T>(string eventName, Action<T> callback)
    {
        _connection.On(eventName, callback);
    }

    public async Task<bool> StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            Console.WriteLine("[INFO] Connection started successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] Starting connection: {ex.Message}");
        }

        return false;
    }
}