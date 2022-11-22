using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendyolBot
{

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ProxyModel
    {
        

       

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }

    public partial class Result
    {
    

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("proxy_address")]
        public string ProxyAddress { get; set; }

        [JsonProperty("port")]
        public long Port { get; set; }


    }
}
