using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    public class Request 
    {
        public string PartitionKey { get; set; }
        public RequestType RequestType { get; set; }
        public string Clientid { get; set; }
        public string Statecd { get; set; }
        public string Username { get; set; }
        public string Txn { get; set; }
        public string ClientSecret { get; set; }
        public string IpUsr { get; set; }
        public int Blob { get; set; }
        public string BlobKey { get; set; }
        public string BlobFile { get; set; }

        public string RequestToken { get; set; }

        public string AuthToken { get; set; }

        public DateTime  CreatedOn { get; set; }

        public string Response { get; set; }

        public string ModifiedOn { get; set; }

        public string RetPeriod { get; set; }

        public string Gstin { get; set; }
        public string Action { get; set; }

        public string ApiAction { get; set; }

        public string param_ctin { get; set; }

        public int param_status_cd { get; set; }

        public string param_ret_period { get; set; }
        public string param_action { get; set; }

        public string param_gstin { get; set; }

        public string param_statecd { get; set; }

        public string param_ref_id { get; set; }

        public string param_token { get; set; }
    }
}
