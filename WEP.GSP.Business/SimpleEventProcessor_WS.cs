using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using WEP.GSP.Document;
using WEP.GSP.Data;
using WEP.GSP.Service.Blob;

namespace WEP.GSP.Business
{
    public class EventHubProcessor:IEventProcessor
    {
        private static int requests = 0;
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Stopping partionid {0}. Reason : {1}",context.PartitionId,reason);
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("Opening partionid {0}.", context.PartitionId);
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine("Error partionid {0}. Error : {1}", context.PartitionId, error);
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            //Await async
            Task.Factory.StartNew(() => EventMessageHandler(messages)).ContinueWith(p =>
            {
                if (p.Exception != null)
                    p.Exception.Handle(x =>
                    {
                        new ExceptionData().InsertExceptionLog(null, x.Message, x.StackTrace, (int)WEP.GSP.Document.Stage.Response_Invoke_ERROR);
                        return false;
                    });
            });
            requests = requests + messages.Count();
            Console.WriteLine("Processing partionid {0}. Total Count : {1}, Current messages : {2}", context.PartitionId, requests , messages.Count());

            //update checkpoint
            return context.CheckpointAsync();
        }

        public void ProcessBatchFile(Request request)
        {
            //Read from service bus and deserialize

            //execute the procedure to split the payload

            //create a table to store splitted details with existing token and no of splits

            //after split move the encrypted json to original service bus

            throw new NotImplementedException();
        }

        private void EventMessageHandler(IEnumerable<EventData> messages)
        {
            
            foreach (var eventData in messages)
            {

                // multiple Task for each messages
                //Await async
                string jsonReqst = new JavaScriptSerializer().Serialize(messages);
                var eventHubMessage = Encoding.UTF8.GetString(eventData.Body.Array
                                                              , eventData.Body.Offset
                                                              , eventData.Body.Count);

                Request request = JsonConvert.DeserializeObject<Request>(eventHubMessage);

                ProcessMessage(jsonReqst, request);
            }
        }
        public void EventMessageHandler(IEnumerable<Microsoft.ServiceBus.Messaging.EventData> messages)
        {
            foreach (var eventData in messages)
            {

                // multiple Task for each messages
                //Await async
                string jsonReqst = new JavaScriptSerializer().Serialize(messages);
                var info = eventData.GetBytes();
                var eventHubMessage = Encoding.UTF8.GetString(info);
                //Task.Factory.StartNew(() => ProcessData(jsonReqst, eventHubMessage));
                Request request = JsonConvert.DeserializeObject<Request>(eventHubMessage);
                ProcessMessage(jsonReqst, request);
            }
        }
        //private async Task ProcessData(string jsonReqst, Request data)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonReqst"></param>
        /// <param name="eventHubMessage"></param>
        private void ProcessMessage(string jsonReqst, Request request)
        {
            try
            {
                //var requestdata = new RequestData();
                //requestdata.InsertBacklogRequest(data.RequestToken, data.Username, "Blob-"+data.Blob );
                //requestdata.InsertRequest(jsonReqst, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Begin);

                GSTR1Business objGSTR1Business = new GSTR1Business(request.Clientid
                                                                    , request.Statecd
                                                                    , request.Username
                                                                    , request.Txn
                                                                    , request.ClientSecret
                                                                    , request.IpUsr
                                                                    , request.AuthToken
                                                                    , request.RetPeriod
                                                                    , request.Gstin);

                //Download the Data from blobstorage 
                ServiceResponse<string> response = objGSTR1Business.ProcessGSTR1(request);

                //Response_Come_From_GSTN in db
                //requestdata.InsertRequest(jsonReqst, data.RequestToken, response.ResponseObject, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Success);
            }
            catch (Exception ex)
            {
                new ExceptionBusiness().InsertExceptionLog(request.RequestToken, ex.Message, ex.StackTrace, (int)Stage.GSTN_Req_API_Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubMessage"></param>
        public void ProcessMessage(Request request)
        {
            
            ServiceResponse<string> response = new Document.ServiceResponse<string>();

            //string inRequestType = request.RequestType.ToString();
            int inRequestTypeValue = (int)request.RequestType;


            GSTR1Business objGSTR1Business = new GSTR1Business(request.Clientid
                                                                    , request.Statecd
                                                                    , request.Username
                                                                    , request.Txn
                                                                    , request.ClientSecret
                                                                    , request.IpUsr
                                                                    , request.AuthToken
                                                                    , request.RetPeriod
                                                                    , request.Gstin);

            GSTR2Business objGSTR2Business = new GSTR2Business(request.Clientid
                                                                   , request.Statecd
                                                                   , request.Username
                                                                   , request.Txn
                                                                   , request.ClientSecret
                                                                   , request.IpUsr
                                                                   , request.AuthToken
                                                                   , request.RetPeriod
                                                                   , request.Gstin);

            GSTR2ABusiness objGSTR2ABusiness = new GSTR2ABusiness(request.Clientid
                                                                   , request.Statecd
                                                                   , request.Username
                                                                   , request.Txn
                                                                   , request.ClientSecret
                                                                   , request.IpUsr
                                                                   , request.AuthToken
                                                                   , request.RetPeriod
                                                                   , request.Gstin);

            GSTR3B_Business objGSTR3B = new GSTR3B_Business(request.Clientid
                                                                   , request.Statecd
                                                                   , request.Username
                                                                   , request.Txn
                                                                   , request.ClientSecret
                                                                   , request.IpUsr
                                                                   , request.AuthToken
                                                                   , request.RetPeriod
                                                                   , request.Gstin);


            //Download the Data from blobstorage 
            switch (inRequestTypeValue)
            {
                //GSTR1
                case (int)RequestType.SaveGSTR1:
                    response = objGSTR1Business.ProcessGSTR1(request);
                    break;

                case (int)RequestType.GSTR1_GetB2B:
                    response = objGSTR1Business.ProcessGSTR1_B2B(request);
                    break;

                case (int)RequestType.GSTR1_GetB2CL:
                    response = objGSTR1Business.ProcessGSTR1_B2CL(request);
                    break;

                case (int)RequestType.GSTR1_GetB2CS:
                    response = objGSTR1Business.ProcessGSTR1_B2CS(request);
                    break;

                case (int)RequestType.GSTR1_Get_NilInvoice:
                    response = objGSTR1Business.ProcessGSTR1_NilInvoice(request);
                    break;

                case (int)RequestType.GSTR1_Get_TxpInvoice:
                    response = objGSTR1Business.ProcessGSTR1_TxpInvoice(request);
                    break;

                case (int)RequestType.GSTR1_Get_AT_Invoice:
                    response = objGSTR1Business.ProcessGSTR1_AT_Invoice(request);
                    break;

                case (int)RequestType.GSTR1_Get_ExpInvoice:
                    response = objGSTR1Business.ProcessGSTR1_ExpInvoice(request);
                    break;

                case (int)RequestType.GSTR1_Get_TrackStatus:
                    response = objGSTR1Business.ProcessGSTR1_TrackStatus(request);
                    break;

                case (int)RequestType.GSTR1_Get_HsnSummary:
                    response = objGSTR1Business.ProcessGSTR1_HsnSummary(request);
                    break;

                case (int)RequestType.GSTR1_Get_CDNR:
                    response = objGSTR1Business.ProcessGSTR1_CdnrSummary(request);
                    break;

                case (int)RequestType.GSTR1_Get_CDNRU:
                    response = objGSTR1Business.ProcessGSTR1_CdnruSummary(request);
                    break;

                case (int)RequestType.GSTR1_Get_DocIssued:
                    response = objGSTR1Business.ProcessGSTR1_DocIssued(request);
                    break;

                case (int)RequestType.GSTR1_Get_Summary:
                    response = objGSTR1Business.ProcessGSTR1_Summary(request);
                    break;

                case (int)RequestType.GSTR1_Get_FileDetails:
                    response = objGSTR1Business.ProcessGSTR1_FileDetails(request);
                    break;

                    //Gstr1 Filing
                case (int)RequestType.GSTR1_File:
                    response = objGSTR1Business.ProcessGSTR1_File(request);
                    break;

                case (int)RequestType.GSTR1_RetSubmit:
                    response = objGSTR1Business.ProcessGSTR1Submit(request);
                    break;

                //GSTR2

                case (int)RequestType.SaveGSTR2:
                    response = objGSTR2Business.ProcessGSTR2Save(request);
                    break;

                case (int)RequestType.GSTR2_GetB2B:
                    response = objGSTR2Business.ProcessGSTR2_B2B(request);
                    break;

                case (int)RequestType.GSTR2_GetCDN:
                    response = objGSTR2Business.ProcessGSTR2_CDN(request);
                    break;

                case (int)RequestType.GSTR2_GetB2BUR:
                    response = objGSTR2Business.ProcessGSTR2_B2BUR(request);
                    break;

                case (int)RequestType.GSTR2_Get_CDNUR:
                    response = objGSTR2Business.ProcessGSTR2_CDNUR(request);
                    break;

                case (int)RequestType.GSTR2_Get_HSNSUM:
                    response = objGSTR2Business.ProcessGSTR2_HsnSummary(request);
                    break;

                case (int)RequestType.GSTR2_Get_TXLI:
                    response = objGSTR2Business.ProcessGSTR2_TaxLiability(request);
                    break;

                case (int)RequestType.GSTR2_Get_TXP:
                    response = objGSTR2Business.ProcessGSTR2_TaxPaidUnderRC(request);
                    break;

                case (int)RequestType.GSTR2_Get_Submit:
                    response = objGSTR2Business.ProcessGSTR2_Submit(request);
                    break;

                case (int)RequestType.GSTR2_Get_Summary:
                    response = objGSTR2Business.ProcessGSTR2_Summary(request);
                    break;

                case (int)RequestType.GSTR2_Get_TrackStatus:
                    response = objGSTR2Business.ProcessGSTR2_TrackStatus(request);
                    break;

                case (int)RequestType.GSTR2_Get_FileDetails:
                    response= objGSTR2Business.ProcessGSTR2_FileDetails(request);
                    break;

                case (int)RequestType.GSTR2_File:
                    response = objGSTR2Business.ProcessGSTR2_File(request);
                    break;

                case (int)RequestType.GSTR2_RetSubmit:
                    response = objGSTR2Business.ProcessGSTR2_RetSubmit(request);
                    break;

                case (int)RequestType.GSTR2_Get_NilInvoice:                   
                    response= objGSTR2Business.ProcessGSTR2_NilInvoice(request);
                    break;

                case (int)RequestType.GSTR2_Get_ImpgInvoice:
                    response = objGSTR2Business.ProcessGSTR2_ImpgInvoice(request);
                    break;

                case (int)RequestType.GSTR2_Get_ImpsInvoice:
                    response = objGSTR2Business.ProcessGSTR2_ImpsInvoice(request);
                    break;

                case (int)RequestType.GSTR2_Get_ItcRvslInvoice:
                    response = objGSTR2Business.ProcessGSTR2_ItcRvslInvoice(request);
                    break;


                //GSTR2A
                case (int)RequestType.GSTR2A_GetB2B:
                    response = objGSTR2ABusiness.ProcessGSTR2A_B2B(request);
                    break;

                
                case (int)RequestType.GSTR2A_GetCDN:
                    response = objGSTR2ABusiness.ProcessGSTR2A_CDN(request);
                    break;

                case (int)RequestType.GSTR2A_Get_FileDetails:
                    response = objGSTR2ABusiness.ProcessGSTR2A_FileDetails(request);
                    break;
               
                //GSTR3B

                case (int)RequestType.SaveGSTR3B:
                    response = objGSTR3B.ProcessGSTR3BSave(request);
                    break;

                case (int)RequestType.GSTR3B_Get_Summary:
                    response = objGSTR3B.ProcessGSTR3BSummary(request);
                    break;

                case (int)RequestType.GSTR3B_RetSubmit:
                    response = objGSTR3B.ProcessGSTR3BSubmit(request);
                    break;

                case (int)RequestType.GSTR3B_RetOffset:
                    response = objGSTR3B.ProcessGSTR3BRetOffset(request);
                    break;

                case (int)RequestType.GSTR3B_RetFile:
                    response = objGSTR3B.ProcessGSTR3BFileData(request);
                    break;

                case (int)RequestType.GSTR3B_Get_TrackStatus:
                    response = objGSTR3B.ProcessGSTR3BTrackStatus(request);
                    break;
            }
            
        }
    }
}
