//using Microsoft.Azure.EventHubs;
//using Microsoft.Azure.EventHubs.Processor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WEP.GSP.Business;
//using WEP.GSP.Document;
//using Newtonsoft.Json;
//using System.Threading;

//namespace GSTN.EventHub.Receiver
//{
//    class Program
//    {

//        public static void Main(string[] args)
//        {
//            Console.WriteLine("Before start thread");


//            MyThread thr1 = new MyThread();
//            MyThread thr2 = new MyThread();

//            Thread tid1 = new Thread(new ThreadStart(thr1.WorkerProcessForLiveRequest));
//            Thread tid2 = new Thread(new ThreadStart(thr2.WorkerProcessForBacklog));

//            tid1.Start();
//            tid2.Start();
//        }

//        //static void Main(string[] args)
//        //{
//        //    try
//        //    {
//        //        MainAsync(args).GetAwaiter().GetResult();

//        //    }
//        //    catch (Exception ex)
//        //    {

//        //        Console.ForegroundColor = ConsoleColor.Red;
//        //        Console.WriteLine("ERROR : " + ex.Message + " | " + ex.InnerException);
//        //        // new RequestData().InsertException(ex.Message, ex.StackTrace, ex.Source, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Failure);
//        //        Console.ResetColor();
//        //    }
//        //}

//        //private static async Task MainAsync(string[] args)
//        //{
//        //    Console.WriteLine("Registering EventProcessor...");

//        //    var eventProcessorHost = new EventProcessorHost(Constants.GstnReqEventHub,
//        //                                                   PartitionReceiver.DefaultConsumerGroupName,
//        //                                                   Constants.GstnReqHubConnectionRead,
//        //                                                   Constants.BlobStorageConnection,
//        //                                                   Constants.BlobContainer);
//        //    // registration fails and retry
//        //    await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor_1_1>();

//        //    Console.WriteLine("Receiving. Press ENTER to stop worker.");
//        //    Console.ReadLine();

//        //    // Disposes of the Event Processor Host
//        //    //On Shutdown unregister the application from event Hub
//        //    // Webhook call to shutdown
//        //    // async health monitoring and log
//        //    //
//        //    await eventProcessorHost.UnregisterEventProcessorAsync();
//        //}
//    }


//    public class MyThread
//    {

//        public void WorkerProcessForLiveRequest()
//        {
//            MainAsync().GetAwaiter().GetResult();
//        }

//        private static async Task MainAsync()
//        {
//            Console.WriteLine("Registering EventProcessor...");

//            var eventProcessorHost = new EventProcessorHost(Constants.GstnReqEventHub,
//                                                           PartitionReceiver.DefaultConsumerGroupName,
//                                                           Constants.GstnReqHubConnectionRead,
//                                                           Constants.BlobStorageConnection,
//                                                           Constants.BlobContainer);
//            // registration fails and retry
//            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor_1_1>();

//            Console.WriteLine("Receiving. Press ENTER to stop worker.");
//            Console.ReadLine();

//            // Disposes of the Event Processor Host
//            //On Shutdown unregister the application from event Hub
//            // Webhook call to shutdown
//            // async health monitoring and log
//            //
//            await eventProcessorHost.UnregisterEventProcessorAsync();
//        }

//        public void WorkerProcessForBacklog()
//        {
//            GSTR1Business objBussiness = new GSTR1Business(); 
//            List<BacklogRequest> backlogReq = objBussiness.GetPendingRequest();

//            if (backlogReq.Count > 0)
//            {
//                foreach (var item in backlogReq)
//                {
//                    objBussiness.RequsetGSTN(item.data);
//                    objBussiness.UpdateResendStatus(item.reqtoken);
//                }
//            }
            
//        }
//    }
//}
