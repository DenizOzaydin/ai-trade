namespace MetuTrade.Business.Settings;

public class AdminSettings
{
    public string BaseAddressUrl { get; set; }
    public string DownloadStartUrl { get; set; }
    public string DownloadOperationsUrl { get; set; }
    public string DownloadCancelUrl { get; set; }
    public string DownloadDeleteCanceledOperationsUrl { get; set; }
    public string DownloadDeleteSucceededOperationsUrl { get; set; }
    public string AdminHubUrl { get; set; }
}
