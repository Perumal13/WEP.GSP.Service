using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using WEP.GSP.Business;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;

namespace WEP.GSP.AZFNC.GSTN
{
    class Program
    {
        //private const string EhConnectionString_Read = "Endpoint=sb://test-eh-namespace.servicebus.windows.net/;SharedAccessKeyName=Read;SharedAccessKey=3s7D+lAO45IBBeowYDYVHY+CeDwgDSLLE7bwe3L30v4=;EntityPath=GSTN-REQ-Event-HUB";
        //private const string EhEntityPath = "GSTN-REQ-Event-HUB";
        //private const string StorageContainerName = "shajeercontainer";
        //public static string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=glamioresourcediag791;AccountKey=fh7AU5L+hQcNoGys0i7DeTDF6IrXDd2+hnBZCYZ1OL+SWpqzwkmAiB8jfL9pfLE3UE9SAVA7C/wpdd+TKfggaA==";

        public static void Main(string[] args)
        {

            //new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
            //                                        , (int)WEP.GSP.Document.Stage.P3_Start_Reading_From_Master_Event_Hub
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , string.Empty
            //                                        , Constants.GSTNStageTable
            //                                        , Constants.currentTime).
            //                                        InsertToTableStorage(string.Empty);

           
                try
                {
                    Console.WriteLine("In While Loop");

                   // GSTR1Business.ReadFromEventHub().GetAwaiter().GetResult();

                    GSTR1Business.ReadFromEventHub().RunSynchronously();
              
                Console.WriteLine("Before Sleep");
                }
                catch(Exception ex)
                {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR : " + ex.Message + " | " + ex.InnerException );
                // new RequestData().InsertException(ex.Message, ex.StackTrace, ex.Source, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Failure);
                Console.ResetColor();
            }
                finally
                {
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("After Sleep");
                }
            
        }


    }

    //public class SimpleEventProcessor : IEventProcessor
    //{
    //    public Task CloseAsync(PartitionContext context, CloseReason reason)
    //    {
    //        Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
    //        return Task.CompletedTask;
    //    }

    //    public Task OpenAsync(PartitionContext context)
    //    {
    //        Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
    //        return Task.CompletedTask;
    //    }

    //    public Task ProcessErrorAsync(PartitionContext context, Exception error)
    //    {
    //        Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
    //        return Task.CompletedTask;
    //    }

    //    public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
    //    {
    //        foreach (var eventData in messages)
    //        {
    //            var myEventHubMessage = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
    //            Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{myEventHubMessage}',Offset:'{context.CheckpointAsync().Id}'");



    //            var data = JsonConvert.DeserializeObject<Request>(myEventHubMessage);

    //            /*new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
    //                                                    , (int)WEP.GSP.Document.Stage.P3_Message_Received_From_Master_Event_Hub
    //                                                    , data.Clientid
    //                                                    , data.Username
    //                                                    , data.ClientSecret
    //                                                    , data.Statecd
    //                                                    , data.Txn
    //                                                    , data.AuthToken
    //                                                    , data.IpUsr
    //                                                    , data.RequestToken
    //                                                    , Constants.AuditLogTable
    //                                                    , Constants.currentTime).
    //                                                    InsertToTableStorage(myEventHubMessage);*/

    //            GSTR1Business objGSTR1Business = new GSTR1Business(data.Clientid, data.Statecd, data.Username, data.Txn, data.ClientSecret, data.IpUsr, data.AuthToken);
    //            ServiceResponse<string> obj = new ServiceResponse<string>();

    //            /*new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
    //                                                    , (int)WEP.GSP.Document.Stage.P3_Request_Sent_To_GSTR1
    //                                                    , data.Clientid
    //                                                    , data.Username
    //                                                    , data.ClientSecret
    //                                                    , data.Statecd
    //                                                    , data.Txn
    //                                                    , data.AuthToken
    //                                                    , data.IpUsr
    //                                                    , data.RequestToken
    //                                                    , Constants.AuditLogTable
    //                                                    , Constants.currentTime).
    //                                                    InsertToTableStorage(string.Empty);*/
    //            obj = objGSTR1Business.ProcessGSTR1(data);

    //            /*new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
    //                                                    , (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN
    //                                                    , data.Clientid
    //                                                    , data.Statecd
    //                                                    , data.Username
    //                                                    , data.Txn
    //                                                    , data.ClientSecret
    //                                                    , data.IpUsr
    //                                                    , data.AuthToken
    //                                                    , data.RequestToken
    //                                                    , Constants.AuditLogTable
    //                                                    , Constants.currentTime).
    //                                                    InsertToTableStorage(string.Empty);*/
    //        }

    //        return context.CheckpointAsync();
    //    }
    //}
}
