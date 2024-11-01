using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Core.ArtificialIntelligence
{
    public class FeatureSettings
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("parameters")]
        public List<object> Parameters { get; set; }
    }
}
