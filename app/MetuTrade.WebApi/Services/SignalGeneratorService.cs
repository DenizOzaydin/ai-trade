using MetuTrade.Business.Results;
using MetuTrade.Core.ArtificialIntelligence;
using MetuTrade.Core.Entities;
using MetuTrade.Core.Miscellaneous;
using MetuTrade.Core.TechnicalAnalysis;
using MetuTrade.DataAccess;
using MetuTrade.WebApi.Hubs;
using MetuTrade.WebApi.Services.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MetuTrade.WebApi.Services
{
    public class SignalGeneratorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Guid, SignalGenerator> _tasks;
        private readonly IHubContext<SignalHub> _signalHub;

        public SignalGeneratorService(IServiceProvider serviceProvider, IHubContext<SignalHub> signalHub)
        {
            _serviceProvider = serviceProvider;
            _tasks = new();
            _signalHub = signalHub;
        }

        public async Task<bool> StartAsync(int botId, string symbol, string interval)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var botRepository = scope.ServiceProvider.GetRequiredService<BotRepository>();

                Bot? bot = await botRepository.GetByIdAsync(botId);

                if (bot == null) return false;
                if (!File.Exists(bot.Url)) return false;

                string json = await File.ReadAllTextAsync(bot.Url);
                NeuralNetwork? network = JsonConvert.DeserializeObject<NeuralNetwork>(json);

                if (network == null) return false;

                Guid taskId = Guid.NewGuid();
                CancellationTokenSource source = new CancellationTokenSource();
                Task task = ProcessAsync(symbol, interval, network, taskId, source.Token);

                SignalGenerator signalGenerator = new SignalGenerator
                {
                    Id = taskId,
                    Symbol = symbol,
                    Interval = interval,
                    Status = OperationStatus.Running,
                    CancellationTokenSource = source,
                    ErrorMessage = "",
                    Task = task
                };

                _tasks.GetOrAdd(signalGenerator.Id, signalGenerator);
            }

            return true;
        }

        public bool Stop(Guid id)
        {
            SignalGenerator? generator;
            bool result = _tasks.TryGetValue(id, out generator);

            if (generator != null)
            {
                generator.CancellationTokenSource.Cancel();
            }

            return result;
        }

        private async Task ProcessAsync(string symbol, string interval, NeuralNetwork network, Guid taskId, CancellationToken token)
        {
            try { 
                while(true)
                {
                    List<Bar> bars = new List<Bar>(); 

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        BarRepository? barRepository = scope.ServiceProvider.GetService<BarRepository>();
                        if (barRepository == null) throw new NullReferenceException("Cannot retrieve required service: BarRepository");
                        bars = await barRepository.GetByFilterAsync(e => e.Symbol == symbol && e.Interval == interval, 7000);
                    }

                    Chart chart = new Chart
                    {
                        Bars = bars,
                        Symbol = symbol,
                        Interval = interval
                    };

                    double? signal = network.Process(chart);
                    if (signal == null)
                    {
                        throw new NullReferenceException("Signal received is null.");
                    }

                    await _signalHub.Clients.All.SendAsync("ReceiveSignal", new SignalResult
                    {
                        Symbol = symbol,
                        Signal = signal,
                        LastUpdated = DateTime.UtcNow
                    });

                    token.ThrowIfCancellationRequested();
                    await Task.Delay(10000);
                }
            }
            catch (OperationCanceledException)
            {
                _tasks[taskId].Status = OperationStatus.Canceled;
            }
            catch (Exception ex)
            {
                _tasks[taskId].Status = OperationStatus.Failure;
                _tasks[taskId].ErrorMessage = ex.Message;
            }
        }

        public List<SignalGenerator> GetAllTasks()
        {
            return _tasks.Values.ToList();
        }
    }
}
