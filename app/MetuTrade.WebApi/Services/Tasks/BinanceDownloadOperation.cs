using MetuTrade.Core.Miscellaneous;

namespace MetuTrade.WebApi.Services.Tasks;

public class BinanceDownloadOperation
{
    public Guid TaskId { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }
    public Task Task { get; set; }
    public OperationStatus Status { get; set; }
    public string ErrorMessage { get; set; }

    public string Symbol { get; set; }
    public string Interval { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long StartTime { get; set; }
    public long CurrentTime { get; set; }
    public long EndTime { get; set; }
    public int PackagesReceived { get; set; } = 0;
}
