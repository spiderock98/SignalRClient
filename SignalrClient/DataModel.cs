namespace SignalrClient;

/// <summary>
/// Have to fix with Model in server-side 
/// </summary>
public class DataModel
{
    public long Time { get; set; }
    public string Key { get; set; } = "";
    public string Classify { get; set; }
    public string Value { get; set; }
}