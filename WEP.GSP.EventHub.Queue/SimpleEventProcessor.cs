using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace WEP.GSP.EventHub.Queue
{
    public class SimpleEventProcessor: IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
            //return null;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            //return Task.CompletedTask;
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            //return Task.CompletedTask;
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {

            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}',CheckPoint:'{context.CheckpointAsync().Id}'");


                //Console.WriteLine($"CheckPointId: '{context.CheckpointAsync().Id}', IsCanceled: '{context.CheckpointAsync().IsCanceled}',IsCompleted: '{context.CheckpointAsync().IsCompleted}'");
                //Console.WriteLine($"IsFaulted: '{context.CheckpointAsync().IsFaulted}', Status: '{context.CheckpointAsync().Status}',Exception: '{context.CheckpointAsync().Exception}'");
            }

            return context.CheckpointAsync();
        }
    }
}
