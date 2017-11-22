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
using Newtonsoft.Json;

namespace WEP.GSP.WindowsServicesSB
{
    public class MyThread
    {
        private static List<QueueClient> clients = new List<QueueClient>();
        public MyThread()
        {

        }

        public void WorkerProcessForLiveRequest()
        {
            for (int i = 0; i < Constants.MaxServiceBusClient; i++)
            {
                clients.Add(QueueClient.CreateFromConnectionString(Constants.GstnReqServiceBusListenPolicyConnectionString, Constants.GstnReqServiceBusQueueName, ReceiveMode.PeekLock));
            }
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {

            EventHubProcessor processor = new EventHubProcessor();

            clients.ForEach(serviceClient => serviceClient
                .OnMessage(message =>
                {
                    var messagestring = message.GetBody<String>();
                    try
                    {
                        processor.ProcessMessage(JsonConvert.DeserializeObject<Request>(messagestring));
                        message.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error :- {0} - {1}", ex.Message, ex.InnerException);
                        message.AbandonAsync();
                    }
                }, new OnMessageOptions() { MaxConcurrentCalls = 100, AutoComplete = false })
                );
        }


    }
}
