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

namespace WEP.GSP.WindowsServices
{
    public class MyThread
    {
        EventProcessorHost eventProcessorHost;
        private static int requests = 0;
        public static bool isRunning = false;
        Microsoft.Azure.EventHubs.Processor.EventProcessorOptions options;
        public MyThread()
        {
            this.options = new Microsoft.Azure.EventHubs.Processor.EventProcessorOptions()
            {

                MaxBatchSize = 100,
                ReceiveTimeout = TimeSpan.FromSeconds(5),
               
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void WorkerProcessForLiveRequest()
        {
            CreateReceiver();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateReceiver()
        {
            if (!isRunning)
            {
                isRunning = true;
                var connectionString = Constants.GstnServiceBusEventHub;
                string eventhubPath = Constants.GstnReqEventHub;
                var nsm = NamespaceManager.CreateFromConnectionString(connectionString);
                //var description = nsm.CreateEventHubIfNotExists(eventhubPath);
                var builder = new ServiceBusConnectionStringBuilder(connectionString)
                {
                    TransportType = TransportType.Amqp
                };
                var factory = MessagingFactory.CreateFromConnectionString(builder.ToString());
                var client = factory.CreateEventHubClient(eventhubPath);
                var group = client.GetDefaultConsumerGroup();
                var requestdata = new WEP.GSP.Data.RequestData();
                try
                {
                    var receiverList = new List<EventHubReceiver>();
                    var availablePartition = requestdata.GetInstancePartitions(System.Environment.MachineName + ConfigurationManager.AppSettings["MachineName"].ToString());
                    if (availablePartition != null && availablePartition.Any())
                    {
                        foreach (var partitionno in availablePartition)
                        {
                            receiverList.Add(group.CreateReceiver(partitionno, requestdata.RetrievePartitionOffset(partitionno)));
                        };

                        var taskFactory = new TaskFactory();

                        var task = (
                        from r in receiverList
                        select taskFactory.StartNew(() =>
                        {
                            Task.Delay(TimeSpan.FromSeconds(1));
                            StartReceiver(Convert.ToInt32(r.PartitionId), r);

                        })).ToList();

                        Task.WaitAll(task.ToArray());
                    }
                    isRunning = false;
                }
                catch (Exception e)
                {
                    isRunning = false;
                    Trace.TraceInformation(e.Message);
                }
            }
            System.Threading.Thread.Sleep(60000);
            CreateReceiver();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverindex"></param>
        /// <param name="r"></param>
        public static void StartReceiver(int receiverindex, EventHubReceiver r)
        {
            var starttime = DateTime.Now;
            bool isMessageAvailable = true;
            var requestdata = new WEP.GSP.Data.RequestData();
            EventHubProcessor processor = new EventHubProcessor();
            do
            {
                try
                {
                    var messages = r.Receive(10);
                    if (messages == null || messages.Count() == 0)
                    {
                        isMessageAvailable = false;
                    }
                    else
                    {
                        Task.Factory.StartNew(() => processor.EventMessageHandler(messages));
                        requestdata.UpdatePartitionOffset(Convert.ToString(receiverindex), messages.Last().Offset);
                        requests = requests + messages.Count();
                    }
                }
                catch (Exception exception)
                {
                    Trace.TraceError("exception on receive {0}", exception.Message);
                }

            } while (isMessageAvailable);
            requestdata.UpdateReleasePartitionLock(Convert.ToString(receiverindex));
        }

        /// <summary>
        /// 
        /// </summary>
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
