using MetuTrade.Core.ArtificialIntelligence;
using MetuTrade.Core.Miscellaneous;

namespace MetuTrade.WebApi.Services.Tasks
{
    public class SignalGenerator
    {
        public Guid Id { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task Task { get; set; }
        public OperationStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public string Symbol { get; set; }
        public string Interval { get; set; }
    }
}
