//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Azure.EventHubs.Processor;
//using Microsoft.Azure.EventHubs;
//using WEP.GSP.Document;
//using Newtonsoft.Json;
//using WEP.GSP.Data;
//using System.Web.Script.Serialization;

//namespace WEP.GSP.Business
//{
//    public class SimpleEventProcessor_1_1 : IEventProcessor
//    {
//        public Task CloseAsync(PartitionContext context, CloseReason reason)
//        {
//            //Console.ForegroundColor = ConsoleColor.Yellow;           
            
//            //Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
//            //Console.ResetColor();
//            return Task.CompletedTask;
//        }

//        public Task OpenAsync(PartitionContext context)
//        {
//            //Console.ForegroundColor = ConsoleColor.Green;
//            //Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
//            //Console.ResetColor();
//            return Task.CompletedTask;
//        }

//        public Task ProcessErrorAsync(PartitionContext context, Exception error)
//        {
//            //Console.ForegroundColor = ConsoleColor.Cyan;
//            //Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
//            //Console.ResetColor();
//            return Task.CompletedTask;
//        }

//        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
//        {
//            Console.ForegroundColor = ConsoleColor.DarkYellow ;
           
//            // Cancellation OR stop of Process with un registration 
//            // Powershell or Webhook
//            foreach (var eventData in messages)
//            {

//                // multiple Task for each messages
//                //Await async

//                string jsonReqst = new JavaScriptSerializer().Serialize(messages);
//                var myEventHubMessage = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
//                var data = JsonConvert.DeserializeObject<Request>(myEventHubMessage);

//                try
//                {
                    
//                    new RequestData().InsertRequest(jsonReqst, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Begin);

                    
//                    Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{myEventHubMessage}',Offset:'{context.CheckpointAsync().Id}'");



                    

//                    GSTR1Business objGSTR1Business = new GSTR1Business(data.Clientid
//                                                                        , data.Statecd
//                                                                        , data.Username
//                                                                        , data.Txn
//                                                                        , data.ClientSecret
//                                                                        , data.IpUsr
//                                                                        , data.AuthToken
//                                                                        ,data.RetPeriod
//                                                                        ,data.Gstin);


//                    //Download the Data from blobstorage 
//                    ServiceResponse<string> obj = objGSTR1Business.ProcessGSTR1(data);




//                    //P4_Response_Come_From_GSTN in db
//                    new RequestData().InsertRequest(jsonReqst, data.RequestToken, data.Response, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Success);
//                    new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
//                                                            , (int)WEP.GSP.Document.Stage.Response_From_GSTN_Success
//                                                            , data.Clientid
//                                                            , data.Statecd
//                                                            , data.Username
//                                                            , data.Txn
//                                                            , data.ClientSecret
//                                                            , data.IpUsr
//                                                            , data.AuthToken
//                                                            , data.RequestToken
//                                                            , Constants.GSTNStageTable
//                                                            , Constants.currentTime).
//                                                            InsertToTableStorage(string.Empty);
//                }
//                catch (Exception ex)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine("ERROR - 1 : " + ex.Message + " | " + ex.InnerException);
//                    Console.ResetColor();
//                    //P4_Response_Come_From_GSTN in db
//                    new ExceptionData().InsertExceptionLog(data.RequestToken, ex.Message, ex.Source, (int)WEP.GSP.Document.Stage.Response_GSTN_Failure);
//                }


//            }
//            Console.ResetColor();
//            return context.CheckpointAsync();

//            #region MyRegion
//            //try
//            //{
//            //    foreach (var eventData in messages)
//            //    {


//            //        string jsonReqst = new JavaScriptSerializer().Serialize(messages);
//            //        new RequestData().InsertRequest(jsonReqst, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Begin);

//            //        var myEventHubMessage = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
//            //        Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{myEventHubMessage}',Offset:'{context.CheckpointAsync().Id}'");



//            //        var data = JsonConvert.DeserializeObject<Request>(myEventHubMessage);







//            //        //Initializing the constructor
//            //        GSTR1Business objGSTR1Business = new GSTR1Business(data.Clientid
//            //                                                            , data.Statecd
//            //                                                            , data.Username
//            //                                                            , data.Txn
//            //                                                            , data.ClientSecret
//            //                                                            , data.IpUsr
//            //                                                            , data.AuthToken);


//            //        //Download the Data from blobstorage 
//            //        ServiceResponse<string> obj = objGSTR1Business.InvokeGSTR1(data);




//            //        //P4_Response_Come_From_GSTN in db
//            //        new RequestData().InsertRequest(jsonReqst, data.RequestToken, data.Response, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Success);
//            //        new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
//            //                                                , (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Success
//            //                                                , data.Clientid
//            //                                                , data.Statecd
//            //                                                , data.Username
//            //                                                , data.Txn
//            //                                                , data.ClientSecret
//            //                                                , data.IpUsr
//            //                                                , data.AuthToken
//            //                                                , data.RequestToken
//            //                                                , Constants.GSTNStageTable
//            //                                                , Constants.currentTime).
//            //                                                InsertToTableStorage(string.Empty);
//            //    }


//            //}
//            //catch(Exception ex)
//            //{
//            //    //P4_Response_Come_From_GSTN in db
//            //    //new RequestData().InsertException(ex.Message, ex.StackTrace, ex.Source, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Failure);
//            //    //new RequestData().InsertExceptionLog(request.RequestToken, ex.Message, ex.Source, (int)WEP.GSP.Document.Stage.Request_Invoke_ERROR);
//            //}

//            //return context.CheckpointAsync();
//            #endregion

//        }
//    }
//}
