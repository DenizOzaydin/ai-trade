namespace MetuTrade.Business.Results;

public class BinanceDownloadResult : ResultBase
{

    public int Count { get; set; }
    public string? Symbol { get; set; }
    public string? Interval { get; set; }
    public long FirstBarOpenTime { get; set; }
    public long? StartTime { get; set; }
    public long? EndTime { get; set; }
    public long? NextStartTime { get; set; }
}