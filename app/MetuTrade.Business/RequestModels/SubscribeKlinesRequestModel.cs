using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.RequestModels
{
    public class SubscribeKlinesRequestModel
    {
        public string Symbol { get; set; }
        public string Interval { get; set; }
    }
}
