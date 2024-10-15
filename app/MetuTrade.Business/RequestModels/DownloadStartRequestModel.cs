namespace MetuTrade.Business.RequestModels;

public class DownloadStartRequestModel
{
    public string? Symbol { get; set; }
    public string? Interval { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}