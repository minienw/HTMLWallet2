namespace CheckInQrWeb.Core;

public class WorkflowResult<T>
{
    public WorkflowResult(T result, bool success, string message, string[] debugMessages)
    {
        Result = result;
        Success = success;
        Message = message;
        DebugMessages = debugMessages;
    }
        
    public T Result { get; }
    public bool Success { get; }
    public string Message { get; }
    public string[] DebugMessages { get; }
}