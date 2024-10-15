namespace MetuTrade.Business.Results;

public class DownloadOperationResult
{
    public Guid TaskId { get; set; }
    public string Status { get; set; }
    public string ErrorMessage { get; set; }

    public string Symbol { get; set; }
    public string Interval { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long StartTime { get; set; }
    public long CurrentTime { get; set; }
    public long EndTime { get; set; }
    public int PackagesReceived { get; set; }
}