using MetuTrade.Business.Results;
using MetuTrade.Business.Services;
using MetuTrade.Core;
using System.Collections.Concurrent;

namespace MetuTrade.WebApi.Services;

public class BinanceBackgroundService
{
    private readonly ConcurrentDictionary<Guid, BinanceDownloadOperation> _tasks;
    private readonly IServiceProvider _serviceProvider;

    public BinanceBackgroundService(IServiceProvider serviceProvider)
    {
        _tasks = new();
        _serviceProvider = serviceProvider;
    }

    public void StartDownload(string symbol, string interval, string startDate, string endDate)
    {
        long startTime = Tools.GetTimestamp(startDate);
        long endTime = Tools.GetTimestamp(endDate);

        Guid taskId = Guid.NewGuid();

        CancellationTokenSource source = new CancellationTokenSource();
        source.CancelAfter(TimeSpan.FromHours(1));
        Task task = DownloadAsync(symbol, interval, startTime, endTime, taskId, source.Token);
        BinanceDownloadOperation operation = new BinanceDownloadOperation
        {
            TaskId = taskId,
            Task = task,
            CancellationTokenSource = source,
            StartDate = startDate,
            EndDate = endDate,
            StartTime = startTime,
            EndTime = endTime,
            CurrentTime = startTime,
            Symbol = symbol,
            Interval = interval,
            Status = OperationStatus.Running
        };

        _tasks.GetOrAdd(operation.TaskId, operation);
    }

    private async Task DownloadAsync(string symbol, string interval, long startTime, long endTime, Guid taskId, CancellationToken token)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                BinanceService binanceService = scope.ServiceProvider.GetRequiredService<BinanceService>();
                long startTime_t = startTime;
                bool flag = false;

                while (true)
                {
                    BinanceDownloadResult result = await binanceService.DownloadAsync(symbol, interval, startTime_t, endTime);
                    if (flag == false)
                    {
                        _tasks[taskId].StartTime = result.FirstBarOpenTime;
                        flag = true;
                    }
                    _tasks[taskId].CurrentTime = result.StartTime ?? _tasks[taskId].CurrentTime;
                    _tasks[taskId].PackagesReceived++;

                    token.ThrowIfCancellationRequested();
                    if (result == null) throw new Exception("Download process return value is null");
                    if (result.StatusCode != System.Net.HttpStatusCode.OK) throw new Exception(result.StatusCode.ToString() + " received");

                    if (result.Count == 0 || result.NextStartTime == null) break;
                    if (result.NextStartTime != null)
                    {
                        startTime_t = (long)result.NextStartTime;
                    }
                }
            }
            _tasks[taskId].Status = OperationStatus.Success;
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

    public bool RequestCancel(Guid taskId)
    {
        BinanceDownloadOperation? operation;
        bool result = _tasks.TryGetValue(taskId, out operation);

        if (operation != null)
        {
            operation.CancellationTokenSource.Cancel();
        }

        return result;
    }

    public void ClearCanceledOperations()
    {
        List<Guid> cancelled = _tasks.Values.Where(task => task.Status == OperationStatus.Canceled).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _tasks.Remove(guid, out _);
        }
    }

    public void ClearSucceededOperations()
    {
        List<Guid> cancelled = _tasks.Values.Where(task => task.Status == OperationStatus.Success).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _tasks.Remove(guid, out _);
        }
    }

    public void ClearFailedOperations()
    {
        List<Guid> cancelled = _tasks.Values.Where(task => task.Status == OperationStatus.Failure).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _tasks.Remove(guid, out _);
        }
    }

    public List<BinanceDownloadOperation> GetAllTasks()
    {
        return _tasks.Values.ToList();
    }
}
