
namespace SignalrClient;

public enum GsmType
{
    Sms,
    Call
}

public enum GmsOperationStatus
{
    Requested, // Server requested.
    InQueue, // Waiting to be processed.
    Handling, // Currently being processed.
    Retrying, // Mark operation failed and Retrying.
    Complete // The operation has been completed.
}

[Serializable]
public class GsmModel
{
    // Unique key GUID
    public string Id { get; set; }

    // Time object created
    public int Created { get; set; }

    // Time server sent requested to client
    public int RequestTime { get; set; }

    // Time process completed
    public int ResponseTime { get; set; }

    // Status of the GSM operation
    public GmsOperationStatus GmsOperationStatus { get; set; }

    // Type of GSM operation
    public GsmType GsmType { get; set; }

    // Phone number associated with the GSM operation
    public string PhoneNumber { get; set; }

    // Message content for SMS operations
    public string Message { get; set; }

    // Number of retry attempts for the operation
    public int Retry { get; set; }

}