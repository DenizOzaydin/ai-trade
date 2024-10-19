using MetuTrade.Business.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.Events
{
    public class DownloadInfoReceivedEventArgs : EventArgs
    {
        public DownloadOperationResult Message { get; set; }

        public DownloadInfoReceivedEventArgs(DownloadOperationResult message)
        {
            Message = message;
        }
    }
}
