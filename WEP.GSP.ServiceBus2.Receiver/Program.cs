using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using WEP.GSP.Business;
using WEP.GSP.Document;
using Newtonsoft.Json;

namespace WEP.GSP.ServiceBus2.Receiver
{
    public class Program
    {
        private static List<QueueClient> clients = new List<QueueClient>();
        private static int count = 0;

        static void Main(string[] args)
        {
            for (int i = 0; i < Constants.MaxServiceBusClient; i++)
            {
                clients.Add(QueueClient.CreateFromConnectionString(Constants.GstnReqServiceBusListenPolicyConnectionString, Constants.GstnReqServiceBusQueueName, ReceiveMode.PeekLock));
            }
            MainServiceBusAsync();
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
                        processor.ProcessBatchFile(JsonConvert.DeserializeObject<Request>(messagestring));
                        count = count + 1;
                        Console.WriteLine("Total Count1 :- {0}", count);
                        message.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Request objRequest = new Request();
                        objRequest = JsonConvert.DeserializeObject<Request>(messagestring);                        
                        message.AbandonAsync();                        
                    }
                }, new OnMessageOptions() { MaxConcurrentCalls = 100, AutoComplete = false })
                );

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();
            clients.ForEach(sbclient => sbclient.Close());
        }
    }
}
