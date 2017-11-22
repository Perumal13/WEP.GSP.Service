using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WEP.GSP.Data;
using WEP.GSP.Document;
using Newtonsoft.Json;

namespace WEP.GSP.EventHub.Queue
{
    public class EventHub : IQueueProcessor
    {
        //private string _partition;
        //private string _connection;
        //private string _entityPath;
        static EventHubClient eventHubClientReq = EventHubClient.CreateFromConnectionString(Constants.GstnReqHubConnectionWrite);
        static EventHubClient eventHubClientResp = EventHubClient.CreateFromConnectionString(Constants.GstnRespHubConnectionWrite);
        public EventHub()
        {
            //this._partition = partition;
            ////this._connection = connection;
            //this._entityPath = entityPath;
        }

      
        //Await
        public bool WriteAsync(string jsonRequest, bool IsRequest)
        {
            if (IsRequest)
            {
                EventData eventData = new EventData(Encoding.UTF8.GetBytes(jsonRequest));
                eventHubClientReq.SendAsync(eventData);
                // task error handling
                return true;
            }
            else
            {
                EventData eventData = new EventData(Encoding.UTF8.GetBytes(jsonRequest));
                eventHubClientResp.SendAsync(eventData);
                return true;
            }
        }

        //public virtual bool Read()
        //{
        //    //MainReadAsync().GetAwaiter().GetResult();
        //    return true;
        //}

        //public virtual bool Write(string jsonRequest)
        //{
        //    //MainReadAsync().GetAwaiter().GetResult();
        //    return true;
        //}
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Azure.EventHubs;
//using Microsoft.Azure.EventHubs.Processor;
//using System.Configuration;
//using WEP.GSP.Document;
//using WEP.GSP.Data;

//namespace WEP.GSP.EventHub.Queue
//{
//    public class EventHub : IQueueProcessor
//    {
//        private string _partition;
//        private string _connection;
//        private string _entityPath;

//        public EventHub(string partition, string connection, string entityPath)
//        {
//            this._partition = partition;
//            this._connection = connection;
//            this._entityPath = entityPath;
//        }

//        //public virtual bool Write(string jsonRequest)
//        //{

//        //    EventHubHelper helper = new EventHubHelper(this._partition
//        //                                            , this._connection
//        //                                            , this._entityPath);

//        //    helper.WriteAsync(jsonRequest).GetAwaiter().GetResult();

//        //    return true;

//        //}

//        //public virtual bool Read()
//        //{
//        //    //MainReadAsync().GetAwaiter().GetResult();
//        //    return true;
//        //}

//        public bool WriteAsync(string jsonRequest,bool IsRequest)
//        {             
//            return true;
//        }
//    }
//}
