using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    [Serializable]
    [DataContract]
    public class Attrbute
    {
        public Attrbute()
        {
        }

        [DataMember]
        public string app_key { get; set; }

        [DataMember]
        public string data { get; set; }

        [DataMember]
        public string hmac { get; set; }

        [DataMember]
        public int status_cd { get; set; }

        [DataMember]
        public string action { get; set; }

        [DataMember]
        public string rek { get; set; }
        [DataMember]
        public string username { get; set; }

        [DataMember]
        public string otp { get; set; }

        [DataMember]
        public string auth_token { get; set; }


        [DataMember]
        public string gstin { get; set; }

        [DataMember]
        public string retperiod { get; set; }


        [DataMember]
        public string actionrequired { get; set; }

        [DataMember]
        public string statecd { get; set; }

        [DataMember]
        public string transid { get; set; }

        [DataMember]
        public string frdt { get; set; }

        [DataMember]
        public string todt { get; set; }

        [DataMember]
        public string rtperiod { get; set; }

        [DataMember]
        public string liabtyp { get; set; }

        [DataMember]
        public string statusfilter { get; set; }

        [DataMember]
        public string ackno { get; set; }
        [DataMember]
        public string fromdate { get; set; }
        [DataMember]
        public string todate { get; set; }

        [DataMember]
        public string sign { get; set; }

        [DataMember]
        public string st { get; set; }

        [DataMember]
        public string sid { get; set; }

        [DataMember]
        public string apiAction { get; set; }

        [DataMember]
        public string password { get; set; }

        // -------------- Download -----------------
        [DataMember]
        public string reqJsonData { get; set; }

        [DataMember]
        public string ErrMsg { get; set; }

        //[DataMember]
        //public HttpResponseMessage resJsonData { get; set; }
        [DataMember]
        public string Userid { get; set; }

        //-------------Parameters-------------------

        [DataMember]
        public string param_ctin { get; set; }

        [DataMember]
        public int param_status_cd { get; set; }

        [DataMember]
        public string param_ret_period { get; set; }

        [DataMember]
        public string param_action { get; set; }

        [DataMember]
        public string param_gstin { get; set; }

        [DataMember]
        public string param_statecd { get; set; }

        [DataMember]
        public string param_ref_id { get; set; }

        [DataMember]
        public string param_token { get; set; }

        ////------------------------------------
        //[DataMember]
        //public string clientid { get; set; }

        //[DataMember]
        //public string clientsecret { get; set; }
    }
}
