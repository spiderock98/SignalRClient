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
                ? $"[ERR][SignalR] Connection closed: {exception.Message}"
                : "[INFO][SignalR] Connection closed without error.");

            await ReConnectHandle(null);
        };

        return connection;
    }

    /// <summary>
    /// Loop retry until the timeout has elapsed or timeout is null (infinite timeout)
    /// </summary>
    private async Task<bool> ReConnectHandle(TimeSpan? timeout)
    {
        var preTick = DateTime.Now;
        var flagTimeout = true;
        
        while (timeout.HasValue == false || DateTime.Now - preTick < timeout.Value)
        {
            if (await StartAsync())
            {
                if (Connected != default)
                {
                    Connected(this, EventArgs.Empty);
                }

                flagTimeout = false;
                break;
            }

            // delay before retry
            await Task.Delay((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
        }

        return !flagTimeout;
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

            Console.WriteLine("[INFO][SignalR] Connection started successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Err][SignalR] Starting connection: {ex.Message}");

            // await ReConnectHandle(TimeSpan.FromMinutes(1));
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
            Console.WriteLine($"[Err][SignalR] {e.Message}");
            return false;
        }
    }
}