using MetuTrade.AdminUI.ViewModels.ChartData;
using MetuTrade.Business.Results;
using MetuTrade.Business.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.AdminUI.Commands.ChartData
{
    public class DownloadOperationsCommand : AsyncCommandBase
    {
        private readonly DownloadChartDataControlViewModel _viewModel;
        private readonly AdminService _adminService;

        public DownloadOperationsCommand(DownloadChartDataControlViewModel viewModel, AdminService adminService)
        {
            _viewModel = viewModel;
            _adminService = adminService;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync()
        {
            List<DownloadOperationResult> result = await _adminService.GetDownloadOperationsAsync();

            _viewModel.DownloadOperationList.Clear();
            foreach (var op in result)
            {
                _viewModel.DownloadOperationList.Add(new DownloadOperationViewModel
                {
                    CurrentTime = op.CurrentTime,
                    EndDate = op.EndDate,
                    EndTime = op.EndTime,
                    ErrorMessage = op.ErrorMessage,
                    Interval = op.Interval,
                    StartDate = op.StartDate,
                    PackagesReceived = op.PackagesReceived,
                    StartTime = op.StartTime,
                    Status = op.Status,
                    Symbol = op.Symbol,
                    TaskId = op.TaskId
                });
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }
    }
}
