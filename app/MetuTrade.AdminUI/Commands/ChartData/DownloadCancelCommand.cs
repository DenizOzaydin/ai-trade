using MetuTrade.AdminUI.ViewModels.ChartData;
using MetuTrade.Business.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.AdminUI.Commands.ChartData
{
    public class DownloadCancelCommand : AsyncCommandBase
    {
        private readonly DownloadChartDataControlViewModel _viewModel;
        private readonly AdminService _adminService;

        public DownloadCancelCommand(DownloadChartDataControlViewModel viewModel, AdminService adminService)
        {
            _viewModel = viewModel;
            _adminService = adminService;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return _viewModel.SelectedDownloadOperation != null && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync()
        {
            await _adminService.DownloadCancelAsync(_viewModel.SelectedDownloadOperation.TaskId);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }
    }
}
