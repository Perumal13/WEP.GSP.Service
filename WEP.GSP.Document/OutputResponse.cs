using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace WepAuthPPIndia.Models
{
    public class OutputResponse
    {
        // --------------------------- Responses --------------------------
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status_cd { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string auth_token { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? expiry { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string sek { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string hmac { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rek { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrMsg { get; set; }


        // Auth Token Responses -------------------------------------

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string token_type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string expires_in { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ext_expires_in { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string not_before { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string resource { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string access_token { get; set; }



        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string error { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string error_description { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int[] error_codes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string timestamp { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string trace_id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string correlation_id { get; set; }

        // Auth Token Responses -------------------------------------
    }
}