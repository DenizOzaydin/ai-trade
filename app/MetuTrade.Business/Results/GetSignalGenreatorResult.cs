using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.Results
{
    public class GetSignalGenreatorResult
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        public string Symbol { get; set; }
        public string Interval { get; set; }
    }
}
