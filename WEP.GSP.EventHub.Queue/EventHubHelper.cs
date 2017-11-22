//using Microsoft.Azure.EventHubs;
//using Microsoft.WindowsAzure.Storage.Shared.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WEP.GSP.Data;
//using WEP.GSP.Document;

//namespace WEP.GSP.EventHub.Queue
//{
//    public class EventHubHelper
//    {
//        private EventHubClient _eventHubClient;
//        string _partionKey;
//        public EventHubHelper(string partionKey, string eventHubConnection, string eventHubEntityPath )
//        {
//            this._partionKey = partionKey;
//            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnection)
//                                                                            {
//                                                                                EntityPath = eventHubEntityPath
//                                                                            };

//            this._eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            
//        }

//        #region EventWrite
//        /// <summary>
//        /// MainAsync
//        /// </summary>
//        /// <returns></returns>
//        //public async Task WriteAsync(string jsonRequest)
//        //{
//        //    await this._eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(jsonRequest)), this._partionKey);
//        //    await this._eventHubClient.CloseAsync();  
//        //}
//        #endregion   
//    }
//}
