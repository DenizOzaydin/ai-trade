using MetuTrade.AdminUI.Commands;
using MetuTrade.AdminUI.Commands.ChartData;
using MetuTrade.Business.Services;
using MetuTrade.Business.WebSocket;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.AdminUI.ViewModels.ChartData
{
    public class DownloadChartDataControlViewModel : ViewModelBase
    {
        private readonly AdminService _adminService;
        private readonly AdminClient _adminClient;

        private string _symbol;
        private string _interval;
        private string _startDate;
        private string _endDate;

        public string Symbol { get { return _symbol; } set { _symbol = value; OnPropertyChanged(nameof(Symbol)); } }
        public string Interval { get { return _interval; } set { _interval = value; OnPropertyChanged(nameof(Interval)); } }
        public string StartDate { get { return _startDate; } set { _startDate = value; OnPropertyChanged(nameof(StartDate)); } }
        public string EndDate { get { return _endDate; } set { _endDate = value; OnPropertyChanged(nameof(EndDate)); } }

        public AsyncCommandBase DownloadButton { get; }
        public AsyncCommandBase GetOperationsButton { get; }
        public AsyncCommandBase DeleteCanceledButton { get; }
        public AsyncCommandBase DeleteSucceededButton { get; }
        public AsyncCommandBase CancelButton { get; }

        public CommandBase SelectionChangedCommand { get; }

        public FullyObservableCollection<DownloadOperationViewModel> DownloadOperationList { get; set; }

        private DownloadOperationViewModel _selectedDownloadOperation;
        public DownloadOperationViewModel SelectedDownloadOperation { get { return _selectedDownloadOperation; } set { _selectedDownloadOperation = value; OnPropertyChanged(nameof(SelectedDownloadOperation)); } }

        private DispatcherQueue _dispatcherQueue;

        public DownloadChartDataControlViewModel(AdminService adminService, AdminClient adminClient)
        {
            _adminService = adminService;
            _adminClient = adminClient;

            DownloadOperationList = new FullyObservableCollection<DownloadOperationViewModel>();
            DownloadOperationList.ItemPropertyChanged += (s, e) =>
            {
                
            };

            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            _adminClient.DownloadInfoReceived += (x, result) =>
            {
                _dispatcherQueue.TryEnqueue(() => {
                    var item = DownloadOperationList.Where(x => x.TaskId == result.Message.TaskId).FirstOrDefault();
                    if (item == null) DownloadOperationList.Add(DownloadOperationViewModel.FromDownloadOperation(result.Message));
                    else
                    {
                        item.PackagesReceived = result.Message.PackagesReceived;
                        item.CurrentTime = result.Message.CurrentTime;
                        item.Status = result.Message.Status;
                    }
                });
            };

            DownloadButton = new DownloadStartCommand(this, _adminService);
            GetOperationsButton = new DownloadOperationsCommand(this, _adminService);
            DeleteCanceledButton = new DownloadDeleteCanceledCommand(this, _adminService);
            DeleteSucceededButton = new DownloadDeleteSucceededCommand(this, _adminService);
            CancelButton = new DownloadCancelCommand(this, _adminService);
        }
    }
}
