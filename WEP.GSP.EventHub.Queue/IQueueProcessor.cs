using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WEP.GSP.EventHub.Queue
{
    public interface IQueueProcessor
    {
        //bool Write(string jsonRequest);

        //bool Read();

        bool WriteAsync(string jsonRequest,bool IsRequest);
    }
}
