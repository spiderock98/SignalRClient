using Microsoft.AspNetCore.SignalR.Client;

namespace SignalrClient;

public class SignalrClientHub
{
    public HubConnection Connection { get; }
    public event EventHandler Connected;

    public SignalrClientHub(string connectionUrl)
    {
        Connection = InitHubConnection(connectionUrl, true);
    }

    public void Register<T>(string eventName, Action<T> callback)
    {
        Connection.On(eventName, callback);
    }

    public async Task<bool> StartConnectionAsync()
    {
        var connStatus = await Start();
        while (!connStatus)
        {
            connStatus = await ReConnectHandle(TimeSpan.FromMinutes(1));
        }

        return true;
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
#if DEBUG
            Console.WriteLine($"[Err][SignalR] {e.Message}");
#endif
            return false;
        }
    }

    #region Private Function

    /// <summary>
    /// Start connecting to server
    /// </summary>
    /// <returns></returns>
    private async Task<bool> Start()
    {
        try
        {
            await Connection.StartAsync();
            if (Connected != default)
            {
                Connected(this, EventArgs.Empty);
            }
#if DEBUG
            Console.WriteLine("[INFO][SignalR] Connection started successfully.");
#endif
            return true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[ERR][SignalR] Starting connection: {ex.Message}");
#endif
        }

        return false;
    }

    /// <summary>
    /// Cretae new connection instance
    /// </summary>
    private HubConnection InitHubConnection(string conn, bool autoreconnect)
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

        if (autoreconnect)
        {
            connection.Closed += async exception =>
            {
#if DEBUG
                Console.WriteLine(exception != default
                    ? $"[ERR][SignalR] Connection closed: {exception.Message}"
                    : "[INFO][SignalR] Connection closed without error.");
#endif
                await ReConnectHandle(null);
            };
        }

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
            if (await Start())
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

    #endregion
}