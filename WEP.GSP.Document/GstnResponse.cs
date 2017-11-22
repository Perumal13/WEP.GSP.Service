using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    //[Serializable]
    public class GstnResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string username { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string reqtoken { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string hmac { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int status_cd { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rek { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Error error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string apiAction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string blobUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int respBlobId { get; set; }

        //public string expiry { get; set; }

        //public string sek { get; set; }
    }
}
