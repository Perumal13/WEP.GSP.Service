using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using WEP.GSP.Business;
using WEP.GSP.Document;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using System.Configuration;

namespace WEP.GSP.WindowsServicesA1
{
    public class MyThread
    {
        EventProcessorHost eventProcessorHost;
        Microsoft.Azure.EventHubs.Processor.EventProcessorOptions options;
        public MyThread()
        {
            this.options = new Microsoft.Azure.EventHubs.Processor.EventProcessorOptions()
            {
                MaxBatchSize = 10,
                ReceiveTimeout = TimeSpan.FromSeconds(5),
               
            };
        }

        public void WorkerProcessForLiveRequest()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            this.eventProcessorHost = new EventProcessorHost(Constants.GstnReqEventHub,
                                                            PartitionReceiver.DefaultConsumerGroupName,
                                                            Constants.GstnReqHubConnectionRead,
                                                            Constants.BlobStorageConnection,
                                                            Constants.EventHubBlobContainer);
            //RegisterEventProcessorAsync
            await this.eventProcessorHost.RegisterEventProcessorAsync<EventHubProcessor>(options);
        }

        public void WorkerProcessForBacklog()
        {
            GSTR1Business objBussiness = new GSTR1Business();
            List<BacklogRequest> backlogRequests = objBussiness.GetPendingRequest();
            if (backlogRequests.Count > 0)
            {
                foreach (var request in backlogRequests)
                {
                    Task.Factory.StartNew(() => objBussiness.ProcessBacklogRequsetGSTN(request));
                }
            }
            System.Threading.Thread.Sleep(60000);
            WorkerProcessForBacklog();
        }

        public void UnRegisterEH()
        {
            this.eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }

    }
}
