using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    [DataContract]
    public class ServiceResponse<T>
    {
        [DataMember]
        public T ResponseObject { get; set; }
        [DataMember]
        public bool IsError { get; set; }
        [DataMember]
        public ExceptionModel ExceptionObject { get; set; }


        public static implicit operator ServiceResponse<T>(ServiceResponse<string> v)
        {
            throw new NotImplementedException();
        }
    }
}
