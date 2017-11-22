//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ServiceBus.Messaging;
//using WEP.GSP.Data;
//using WEP.GSP.Document;
//using Newtonsoft.Json;

//namespace WEP.GSP.EventHub.Queue
//{
//    public class EventHub : IQueueProcessor
//    {
//        //private string _partition;
//        //private string _connection;
//        //private string _entityPath;
//        static EventHubClient eventHubClientReq = EventHubClient.CreateFromConnectionString(Constants.GstnReqHubConnectionWrite);
//        static EventHubClient eventHubClientResp = EventHubClient.CreateFromConnectionString(Constants.GstnRespHubConnectionWrite);
//        public EventHub( )
//        {
//            //this._partition = partition;
//            ////this._connection = connection;
//            //this._entityPath = entityPath;
//        }

//        public bool WriteAsync(string jsonRequest, bool IsRequest)
//        {    if (IsRequest)
//            {
//                EventData eventData = new EventData(Encoding.UTF8.GetBytes(jsonRequest));
//                eventHubClientReq.SendAsync(eventData);
//                return true;
//            }
//            else
//            {
//                EventData eventData = new EventData(Encoding.UTF8.GetBytes(jsonRequest));
//                eventHubClientResp.SendAsync(eventData);
//                return true;
//            }
//        }

//        //public virtual bool Read()
//        //{
//        //    //MainReadAsync().GetAwaiter().GetResult();
//        //    return true;
//        //}

//        //public virtual bool Write(string jsonRequest)
//        //{
//        //    //MainReadAsync().GetAwaiter().GetResult();
//        //    return true;
//        //}
//    }
//}
