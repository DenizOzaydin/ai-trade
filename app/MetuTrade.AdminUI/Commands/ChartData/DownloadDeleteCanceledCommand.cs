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
    public class DownloadDeleteCanceledCommand : AsyncCommandBase
    {
        private readonly DownloadChartDataControlViewModel _viewModel;
        private readonly AdminService _adminService;

        public DownloadDeleteCanceledCommand(DownloadChartDataControlViewModel viewModel, AdminService adminService)
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
            await _adminService.DeleteCanceledOperationsAsync();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }
    }
}
