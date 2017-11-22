using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    [DataContract]
    public class CustomException : Exception
    {
        public CustomException(string error)
        {
            Error = error;
        }
        public CustomException(string error, int errorCode)
        {
            Error = error;
        }
        public CustomException(string error,string ErrorSource , int errorCode)
        {
            Error = error;
        }        

        [DataMember]
        public string Error { get; set; }
        public int ErrorCode { get; set; }

        public string ErrorSource { get; set; }

    }
}
