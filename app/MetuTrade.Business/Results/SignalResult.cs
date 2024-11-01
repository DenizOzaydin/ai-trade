using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.Results
{
    public class SignalResult
    {
        public string Symbol { get; set; }
        public double? Signal { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
