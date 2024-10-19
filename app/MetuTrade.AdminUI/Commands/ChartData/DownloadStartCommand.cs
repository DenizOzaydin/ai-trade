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
    public class DownloadStartCommand : AsyncCommandBase
    {
        private readonly DownloadChartDataControlViewModel _viewModel;
        private readonly AdminService _adminService;

        public DownloadStartCommand(DownloadChartDataControlViewModel viewModel, AdminService adminService)
        {
            _viewModel = viewModel;
            _adminService = adminService;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_viewModel.Symbol)
                && !string.IsNullOrEmpty(_viewModel.Interval)
                && !string.IsNullOrEmpty(_viewModel.StartDate)
                && !string.IsNullOrEmpty(_viewModel.EndDate)
                && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync()
        {
            DownloadStartResult result = await _adminService.DownloadStartAsync(_viewModel.Symbol, _viewModel.Interval, _viewModel.StartDate, _viewModel.EndDate);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }
    }
}
