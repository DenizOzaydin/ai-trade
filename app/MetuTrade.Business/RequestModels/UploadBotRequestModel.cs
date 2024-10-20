using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.RequestModels
{
    public class UploadBotRequestModel
    {
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
