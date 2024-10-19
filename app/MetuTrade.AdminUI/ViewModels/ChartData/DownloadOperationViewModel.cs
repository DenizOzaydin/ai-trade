using MetuTrade.Business.Results;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MetuTrade.AdminUI.ViewModels.ChartData
{
    public class DownloadOperationViewModel : ViewModelBase
    {
        public Guid TaskId { get; set; }
        public string ErrorMessage { get; set; }

        private string _status;
        private long _currentTime;
        private int _packagesReceived;

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(nameof(Status)); } }
        public long CurrentTime { get { return _currentTime; } set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); } }
        public int PackagesReceived { get { return _packagesReceived; } set { _packagesReceived = value; OnPropertyChanged(nameof(PackagesReceived)); } }

        public string Symbol { get; set; }
        public string Interval { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public string Description => Symbol + " " + Interval;
        public double Ratio => Status == "Success" ? 100.0 : (EndTime - StartTime != 0 ? (CurrentTime - StartTime) * 100.0 / (EndTime - StartTime) : 0);
        public string RatioString => Ratio.ToString("0.00") + "%";
        public Brush RatioColor => new SolidColorBrush(Status == "Success" ? Color.FromArgb(255, 0, 255, 0) : Status == "Failure" ? Color.FromArgb(255, 255, 0, 0) : Status == "Cancelled" ? Color.FromArgb(255, 127, 127, 127) : Color.FromArgb(255, 255, 255, 0));
    
        public static DownloadOperationViewModel FromDownloadOperation(DownloadOperationResult result)
        {
            DownloadOperationViewModel model = new DownloadOperationViewModel
            {
                CurrentTime = result.CurrentTime,
                EndDate = result.EndDate,
                EndTime = result.EndTime,
                ErrorMessage = result.ErrorMessage,
                Interval = result.Interval,
                PackagesReceived = result.PackagesReceived,
                StartDate = result.StartDate,
                StartTime = result.StartTime,
                Symbol = result.Symbol,
                Status = result.Status,
                TaskId = result.TaskId
            };
            return model;
        }
    }
}
