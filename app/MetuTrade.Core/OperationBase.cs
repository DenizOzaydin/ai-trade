namespace MetuTrade.Core;

public class OperationBase
{
    public Guid TaskId { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }
    public Task Task { get; set; }
    public OperationStatus Status { get; set; }
    public string ErrorMessage { get; set; }
}