using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using WEP.GSP.Business;
using WEP.GSP.Document;

using System.Diagnostics;
using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using System.Configuration;
using Newtonsoft.Json;

namespace WEP.GSP.Receiver
{
    class Program
    {
        private static List<QueueClient> clients = new List<QueueClient>();
        private static int count = 0;

        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i < Constants.MaxServiceBusClient; i++)
                {
                    clients.Add(QueueClient.CreateFromConnectionString(Constants.GstnReqServiceBusListenPolicyConnectionString, Constants.GstnReqServiceBusQueueName, ReceiveMode.PeekLock));
                }
                MainServiceBusAsync();
            }
            catch (Exception ex)
            {
            }
            //MainAsync().GetAwaiter().GetResult();

            


        }

        private static async Task MainAsync()
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(Constants.GstnReqEventHub,
                                                           PartitionReceiver.DefaultConsumerGroupName,
                                                           Constants.GstnReqHubConnectionRead,
                                                           Constants.BlobStorageConnection,
                                                           "gstn-req-event-hub-container");
            //var eventProcessorHost = new EventProcessorHost(ConfigurationManager.AppSettings["GstnReqEventHub"].ToString(),
            //                                              PartitionReceiver.DefaultConsumerGroupName,
            //                                               ConfigurationManager.AppSettings["GstnReqHubConnectionRead"].ToString(),
            //                                              ConfigurationManager.AppSettings["BlobStorageConnection"].ToString(),
            //                                              ConfigurationManager.AppSettings["BlobContainer"].ToString());

            // registration fails and retry
            var options = new Microsoft.Azure.EventHubs.Processor.EventProcessorOptions()
            {

                MaxBatchSize = 10,
                ReceiveTimeout = TimeSpan.FromSeconds(5),
                //InitialOffsetProvider = (partitionId) => "4312342208",
                //InitialOffsetProvider = (partitionId) => 98585,
            };
            await eventProcessorHost.RegisterEventProcessorAsync<EventHubProcessor>(options);

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            //On Shutdown unregister the application from event Hub
            // Webhook call to shutdown
            // async health monitoring and log
            //
            await eventProcessorHost.UnregisterEventProcessorAsync();

        }

        private static async Task MainServiceBusAsync()
        {

            EventHubProcessor processor = new EventHubProcessor();

            clients.ForEach(serviceClient => serviceClient
                .OnMessage(message =>
            {
                var messagestring = message.GetBody<String>();
                try
                {
                    processor.ProcessMessage(JsonConvert.DeserializeObject<Request>(messagestring));
                    //var a = 0;
                    //var i = 1 / a;
                    count = count + 1;
                    Console.WriteLine("Total Count1 :- {0}", count);
                    message.CompleteAsync();
                }
                catch (Exception ex)
                {
                    Request objRequest = new Request();
                    objRequest = JsonConvert.DeserializeObject<Request>(messagestring);
                    //Console.WriteLine("Error :- {0} - {1}", ex.Message, ex.InnerException);
                    //Console.WriteLine("Error :- {0}", messagestring);
                    //Console.WriteLine("Error :- {0} - {1} - {2} -{3}", ex.Message, objRequest.BlobFile,objRequest.Blob,objRequest.ApiAction);
                    message.AbandonAsync();

                    
                    //new  InsertExceptionLog(ex.Message, busMessage.BlobFile);

                }
            }, new OnMessageOptions() { MaxConcurrentCalls = 100, AutoComplete = false })
                );
            
            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();
            clients.ForEach(sbclient => sbclient.Close());
        }
        async static void ProcessReceivedMessage(Task<BrokeredMessage> t)
        {
            EventHubProcessor processor = new EventHubProcessor();

            BrokeredMessage message = t.Result;
            if (message != null)
            {
                var messagestring = message.GetBody<String>();
                try
                {
                    processor.ProcessMessage(JsonConvert.DeserializeObject<Request>(messagestring));
                    //var a = 0;
                    //var i = 1 / a;
                    count = count + 1;
                    Console.WriteLine("Total Count :- {0}", count);
                    message.CompleteAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error :- {0} - {1}", ex.Message, ex.InnerException);
                    message.Abandon();
                }
            }
        }
    }
}

