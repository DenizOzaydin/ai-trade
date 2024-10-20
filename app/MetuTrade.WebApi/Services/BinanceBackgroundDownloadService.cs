using MetuTrade.Business.Results;
using MetuTrade.Business.Services;
using MetuTrade.Core;
using MetuTrade.Core.Miscellaneous;
using MetuTrade.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MetuTrade.WebApi.Services;

public class BinanceBackgroundDownloadService
{
    private readonly ConcurrentDictionary<Guid, BinanceDownloadOperation> _downloadOperations;

    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<AdminHub> _adminHub;

    public BinanceBackgroundDownloadService(IServiceProvider serviceProvider, IHubContext<AdminHub> adminHub)
    {
        _downloadOperations = new();
        _serviceProvider = serviceProvider;
        _adminHub = adminHub;
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

        _downloadOperations.GetOrAdd(operation.TaskId, operation);
    }

    private async Task DownloadAsync(string symbol, string interval, long startTime, long endTime, Guid taskId, CancellationToken token)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                BinanceHttpService binanceService = scope.ServiceProvider.GetRequiredService<BinanceHttpService>();
                long startTime_t = startTime;
                bool flag = false;

                while (true)
                {
                    BinanceDownloadResult result = await binanceService.DownloadAsync(symbol, interval, startTime_t, endTime);
                    if (flag == false)
                    {
                        _downloadOperations[taskId].StartTime = result.FirstBarOpenTime;
                        flag = true;
                    }
                    _downloadOperations[taskId].CurrentTime = result.StartTime ?? _downloadOperations[taskId].CurrentTime;
                    _downloadOperations[taskId].PackagesReceived++;

                    DownloadOperationResult dor = new DownloadOperationResult
                    {
                        CurrentTime = _downloadOperations[taskId].CurrentTime,
                        EndDate = _downloadOperations[taskId].EndDate,
                        EndTime = _downloadOperations[taskId].EndTime,
                        ErrorMessage = _downloadOperations[taskId].ErrorMessage,
                        Interval = _downloadOperations[taskId].Interval,
                        TaskId = taskId,
                        PackagesReceived = _downloadOperations[taskId].PackagesReceived,
                        StartDate = _downloadOperations[taskId].StartDate,
                        StartTime = _downloadOperations[taskId].StartTime,
                        Status = _downloadOperations[taskId].Status.ToString(),
                        Symbol = _downloadOperations[taskId].Symbol
                    };

                    await _adminHub.Clients.All.SendAsync("ReceiveDownloadOperationMessage", dor);

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
            _downloadOperations[taskId].Status = OperationStatus.Success;
        }
        catch (OperationCanceledException)
        {
            _downloadOperations[taskId].Status = OperationStatus.Canceled;
        }
        catch (Exception ex)
        {
            _downloadOperations[taskId].Status = OperationStatus.Failure;
            _downloadOperations[taskId].ErrorMessage = ex.Message;
        }
    }

    public bool RequestCancel(Guid taskId)
    {
        BinanceDownloadOperation? operation;
        bool result = _downloadOperations.TryGetValue(taskId, out operation);

        if (operation != null)
        {
            operation.CancellationTokenSource.Cancel();
        }

        return result;
    }

    public void ClearCanceledOperations()
    {
        List<Guid> cancelled = _downloadOperations.Values.Where(task => task.Status == OperationStatus.Canceled).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _downloadOperations.Remove(guid, out _);
        }
    }

    public void ClearSucceededOperations()
    {
        List<Guid> cancelled = _downloadOperations.Values.Where(task => task.Status == OperationStatus.Success).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _downloadOperations.Remove(guid, out _);
        }
    }

    public void ClearFailedOperations()
    {
        List<Guid> cancelled = _downloadOperations.Values.Where(task => task.Status == OperationStatus.Failure).Select(task => task.TaskId).ToList();
        foreach (var guid in cancelled)
        {
            _downloadOperations.Remove(guid, out _);
        }
    }

    public List<BinanceDownloadOperation> GetAllTasks()
    {
        return _downloadOperations.Values.ToList();
    }
}
