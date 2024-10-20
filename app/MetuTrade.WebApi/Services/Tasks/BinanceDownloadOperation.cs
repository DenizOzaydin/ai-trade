using MetuTrade.Core.Miscellaneous;

namespace MetuTrade.WebApi.Services.Tasks;

public class BinanceDownloadOperation : OperationBase
{
    public string Symbol { get; set; }
    public string Interval { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public long StartTime { get; set; }
    public long CurrentTime { get; set; }
    public long EndTime { get; set; }
    public int PackagesReceived { get; set; } = 0;
}
