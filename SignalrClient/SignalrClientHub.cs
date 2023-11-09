using Microsoft.AspNetCore.SignalR.Client;

namespace SignalrClient;

public class SignalrClientHub
{
    public HubConnection Connection { get; }
    public event EventHandler Connected;

    public SignalrClientHub(string connectionUrl)
    {
        Connection = InitHubConnection(connectionUrl);
    }

    private HubConnection InitHubConnection(string conn)
    {
        // Configure and build the HubConnection
        var connection = new HubConnectionBuilder()
            .WithUrl(conn, options =>
            {
                // options.Headers["Authorization"] =
                //     "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiU3lzdGVtIiwibmJmIjoxNjk2OTEzODc0LCJleHAiOjE2OTc1MTg2NzQsImlhdCI6MTY5NjkxMzg3NH0.-4Gd7JFSDoaJCPbECMPvZezIGz2AdwRWMc4NALczdxg";

                // options.AccessTokenProvider = () => Task.FromResult(
                //         "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiU3lzdGVtIiwibmJmIjoxNjk2OTEzODc0LCJleHAiOjE2OTc1MTg2NzQsImlhdCI6MTY5NjkxMzg3NH0.-4Gd7JFSDoaJCPbECMPvZezIGz2AdwRWMc4NALczdxg");
            })
            // .WithAutomaticReconnect()
            .Build();

        connection.Closed += async exception =>
        {
            Console.WriteLine(exception != default
                ? $"[ERR] Connection closed: {exception.Message}"
                : "[INFO] Connection closed without error.");

            while (true)
            {
                if (await StartAsync())
                {
                    if (Connected != default)
                    {
                        Connected(this, EventArgs.Empty);
                    }

                    break;
                }

                await Task.Delay((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
            }
        };

        return connection;
    }

    public void Register<T>(string eventName, Action<T> callback)
    {
        Connection.On(eventName, callback);
    }

    public async Task<bool> StartAsync()
    {
        try
        {
            await Connection.StartAsync();
            if (Connected != default)
            {
                Connected(this, EventArgs.Empty);
            }

            Console.WriteLine("[INFO] Connection started successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] Starting connection: {ex.Message}");
        }

        return false;
    }

    public async Task<bool> SendMessage<T>(string methodName, T msg)
    {
        try
        {
            await Connection.SendAsync(methodName, msg);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}