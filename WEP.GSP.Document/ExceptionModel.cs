using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    [DataContract]
    public class ExceptionModel
    {
        [DataMember]
        public string ErrorMessage { get; set; }
        //[DataMember]
        //public string StackTrace { get; set; }
        [DataMember]
        public int Severity { get; set; }
        [DataMember]
        public string[] KeyParameter { get; set; }
        [DataMember]
        public string Source { get; set; }
    }

    //[DataContract]
    //public class CustomException : Exception
    //{
    //    public CustomException(string error)
    //    {
    //        Error = error;
    //    }
    //    [DataMember]
    //    public string Error { get; set; }

    //}
}
