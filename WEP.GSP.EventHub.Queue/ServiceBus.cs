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
    public class ServiceBus : IQueueProcessor
    {
        private static string connectionString = Constants.GstnReqServiceBusSendPolicyConnectionString;
        private static string queueName = Constants.GstnReqServiceBusQueueName;
        private static QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.PeekLock);
        public virtual bool Write(string jsonRequest)
        {
            var message = new BrokeredMessage(jsonRequest);
            client.Send(message);
            return true;
        }
        public virtual bool Read()
        {
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            
            client.OnMessage(message =>
            {
                var messagebody = message.GetBody<String>();
                var messageId = message.MessageId;
            });

            Console.ReadLine();
            return true;
        }

        public virtual bool WriteAsync(string jsonRequest, bool IsRequest)
        {
            var message = new BrokeredMessage(jsonRequest);
            client.SendAsync(message);
            return true;
        }
    }
   
}
