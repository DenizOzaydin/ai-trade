namespace MetuTrade.WebApi.Services.Models
{
    public class BinanceSubscriptionMessage
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
}
