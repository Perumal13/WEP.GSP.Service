using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    public class Constants
    {
 

        //Api Url
        public static string ApiGSTR1 = ConfigurationManager.AppSettings["GST_GSTR1"].ToString();
        public static string ApiGSTR2 = ConfigurationManager.AppSettings["GST_GSTR2"].ToString();
        public static string ApiGSTR2A = ConfigurationManager.AppSettings["GST_GSTR2A"].ToString();
        public static string ApiGSTR3B = ConfigurationManager.AppSettings["GST_GSTR3B"].ToString();

        public static string ApiGSTR1_TrackStatus= ConfigurationManager.AppSettings["GST_GSTR1_TrackStatus"].ToString();
        public static string ApiGSTR2_TrackStatus = ConfigurationManager.AppSettings["GST_GSTR2_TrackStatus"].ToString();

        //till ~/returns
        public static string ApiCommon = ConfigurationManager.AppSettings["ApiCommon"].ToString();

        //Authentication Url
        public static string AuthenticateRequest = ConfigurationManager.AppSettings["UserAuthenticate"].ToString();

        //Common API
        public static string CommonApiAuthenticate = ConfigurationManager.AppSettings["CommonAuthentication"].ToString();
        public static string TaxPayerSearch = ConfigurationManager.AppSettings["TaxPayerSearch"].ToString();

        //Mock Response
        public static string ApiGSTR1_Common = ConfigurationManager.AppSettings["GST_GSTR1_Common"].ToString();
        public static string ApiGSTR1_B2B = ConfigurationManager.AppSettings["GST_GSTR1_B2B"].ToString();
        public static string ApiGSTR1_B2CL = ConfigurationManager.AppSettings["GST_GSTR1_B2CL"].ToString();

        public void Api_GSTR1()
        {
            ApiGSTR1 = ConfigurationManager.AppSettings["GST_GSTR1"].ToString();
            ApiGSTR2 = ConfigurationManager.AppSettings["GST_GSTR2"].ToString();
            ApiGSTR2A = ConfigurationManager.AppSettings["GST_GSTR2A"].ToString();
            ApiGSTR3B= ConfigurationManager.AppSettings["GST_GSTR3B"].ToString();

            ApiGSTR1_TrackStatus = ConfigurationManager.AppSettings["GST_GSTR1_TrackStatus"].ToString();
            AuthenticateRequest= ConfigurationManager.AppSettings["UserAuthenticate"].ToString();
            ApiGSTR2_TrackStatus= ConfigurationManager.AppSettings["GST_GSTR2_TrackStatus"].ToString();

            CommonApiAuthenticate= ConfigurationManager.AppSettings["CommonAuthentication"].ToString();
            TaxPayerSearch = ConfigurationManager.AppSettings["TaxPayerSearch"].ToString();

            ApiGSTR1_Common = ConfigurationManager.AppSettings["GST_GSTR1_Common"].ToString();
            ApiGSTR1_B2B = ConfigurationManager.AppSettings["GST_GSTR1_B2B"].ToString();
            ApiGSTR1_B2CL = ConfigurationManager.AppSettings["GST_GSTR1_B2CL"].ToString();
        }

        //TableStorage Connection String
        public static string TableStorageConnection = ConfigurationManager.AppSettings["TableStorageConnection"].ToString();
        public static string TableStorageConnection1 = ConfigurationManager.AppSettings["TableStorageConnection1"].ToString();
        //Table Name
        public static string GSTNStageTable = "GSTNStage";
        public static string GSTNStageP1Table = "GSTNStageP1";
        public static string GSTNStageP2Table = "GSTNStageP2";
        public static string GSTNAuditTable = "GSTNAudit";
        public static string GSTNException = "GSTNException";
        public static string GSTR2AStageTable = "GSTR2AStage";
        public static string GSTR2StageTable = "GSTR2Stage";
        //Blob Event hub Offset //leaseContainerName
        public static string BlobStorageConnection = ConfigurationManager.AppSettings["BlobStorageConnection"].ToString();
        public static string BlobContainer = ConfigurationManager.AppSettings["BlobContainer"].ToString();

   

        //TableStorage PartitionKey and Rowkey
        public static string PartitionKey = DateTime.Now.ToString("MMMMyyyy");
        public static string RowKey = DateTime.Now.Ticks.ToString();
        public static string currentTime = DateTime.Now.Ticks.ToString();
        public static string actualTime = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss.fff");


        //public static string guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
        //public static string RowKey = guid.Substring(0, 18);

        // Blob
        public static string CotainerPayload = "payload";// 
        public static string GstnResponseContainer = "gstnresponse";

        //EventHub
        //public static string MasterHubConnectionWrite = ConfigurationManager.AppSettings["MasterHubConnectionWrite"].ToString();

        public static string GstnRespHubConnectionWrite = ConfigurationManager.AppSettings["GstnRespHubConnectionWrite"].ToString();

        public static string GstnReqHubConnectionWrite = ConfigurationManager.AppSettings["GstnReqHubConnectionWrite"].ToString();
        public static string GstnReqHubConnectionRead = ConfigurationManager.AppSettings["GstnReqHubConnectionRead"].ToString();

        //public static string MasterEventHub  = ConfigurationManager.AppSettings["MasterEventHub"].ToString();
        public static string GstnRespEventHub = ConfigurationManager.AppSettings["GstnRespEventHub"].ToString();
        public static string GstnReqEventHub = ConfigurationManager.AppSettings["GstnReqEventHub"].ToString();
        public static string GstnServiceBusEventHub = ConfigurationManager.AppSettings["ServiceBus.Eventhub.ConnectionString"].ToString();
        public static string EventHubBlobContainer = ConfigurationManager.AppSettings["EventHubBlobContainer"].ToString();
        //partionKey
        public static string PK_SaveGSTR1 = "PK_SaveGSTR1";
        public static string PK_GSTR1_REQ = "PK_GSTR1_REQ";
        public static string PK_GSTR1_RES = "PK_GSTR1_RES";
        //public static string AvailablePartition = ConfigurationManager.AppSettings["AvailablePartition"].ToString();
        // 
        public static int PayloadSize = Convert.ToInt32(ConfigurationManager.AppSettings["Payload_Size"]);

        //Service Bus
        public static string GstnReqServiceBusName = ConfigurationManager.AppSettings["GstnReqServiceBusName"] ==null? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusName"];
        public static string GstnReqServiceBusQueueName = ConfigurationManager.AppSettings["GstnReqServiceBusQueueName"] == null ? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusQueueName"];
        public static string GstnReqServiceBusSendPolicyName = ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyName"] ==null? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyName"];
        public static string GstnReqServiceBusSendPolicyKey = ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyKey"] ==null? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyKey"];
        public static string GstnReqServiceBusSendPolicyConnectionString = ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyConnectionString"] ==null? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusSendPolicyConnectionString"];
        public static string GstnReqServiceBusListenPolicyName = ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyName"] == null ? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyName"];
        public static string GstnReqServiceBusListenPolicyKey = ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyKey"] == null ? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyKey"];
        public static string GstnReqServiceBusListenPolicyConnectionString = ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyConnectionString"] == null ? string.Empty : ConfigurationManager.AppSettings["GstnReqServiceBusListenPolicyConnectionString"];
        public static int MaxServiceBusClient = ConfigurationManager.AppSettings["MaxServiceBusClient"] == null ? 1 : Convert.ToInt32( ConfigurationManager.AppSettings["MaxServiceBusClient"]);


        // retry 
        public static int MaxTrial = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTrial"]);
        public static int MaxTrialP1 = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTrialP1"]);
        public static int MaxBlobDownloadTrial = Convert.ToInt32(ConfigurationManager.AppSettings["MaxBlobDownloadTrial"]);
        public static int DelayTrialms = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTrialms"]);
        public static int DelayTrialmsP1 = Convert.ToInt32(ConfigurationManager.AppSettings["DelayTrialmsP1"]);
        public static string RetryHttpError = ConfigurationManager.AppSettings["RetryHttpError"].ToString();
         
        //Database
        public const string GSPConnect = "GSPConnect";

        //Header info
        public static string UserName = "username";  
        public static string Auth_Token = "auth-token";
        public static string Clientid = "clientid";
        public static string State_cd = "state-cd";
        public static string Txn = "txn";
        public static string Client_secret ="client-secret";
        public static string IpUsr = "ip-usr";
        public static string Ret_period = "ret_period";
        public static string Gstin = "gstin";

        public const string ContentType = "application/json";
        public const string Accept = "application/json";
        public const string PUT = "PUT";
        public const string POST = "POST";
        public const string GET = "GET";
        public const string Status__cd = "status_cd";
        public const string Data = "data";
        public const string Hmac = "hmac";
        public const string Rek = "rek";

        //Queue
        public const string QueueIn = "Queue-IN";
        public const string QueueOut = "Queue-OUT";
        public const string QueueStatus = "StatusQueue";
        public const string QueueAudit = "AuditQueue";
        public const string QueueStage = "StageQueue";
        public const string QueueError = "ErrorQueue";

        //Error
        public const string ErrInputPropervalue = "Please input proper value";
        public const string ErrInputBodyvalue = "Please provide proper value body";
        public const string ErrAspUserOrAuthToken = "Please provide ASP UserId or AuthToken";

        // Yogesh
        // 99866 71800

        //Query strings
        public const string querystringAction = "action";
        public const string querystringGstin = "gstin";
        public const string querystringRet_period = "ret_period";
        public const string querystringCtin = "ctin";
        public const string querystringState_cd = "state_cd";
        public const string querystringRef_id = "ref_id";
        public const string querystringToken = "token";

        //Action Gstr1
        public const string actionGSTR1_SaveGSTR1 = "GSTR1_Save";
        public const string actionGSTR1_GetB2B = "GSTR1_GetB2B";
        public const string actionGSTR1_GetB2CL = "GSTR1_GetB2CL";
        public const string actionGSTR1_GetB2CS = "GSTR1_GetB2CS";
        public const string actionGSTR1_GetNilInvoices = "GSTR1_GetNilInvoices";
        public const string actionGSTR1_GetTxpInvoices = "GSTR1_GetTxpInvoices";
        public const string actionGSTR1_GetAtInvoices = "GSTR1_GetAtInvoices";
        public const string actionGSTR1_GetExpInvoices = "GSTR1_GetExpInvoices";
        public const string actionGSTR1_GetTrackStatus = "GSTR1_GetTrackStatus";
        public const string actionGSTR1_GetHsnSummary = "GSTR1_GetHsnSummary";
        public const string actionGSTR1_GetCDNR = "GSTR1_GetCDNR";
        public const string actionGSTR1_GetCDNRU = "GSTR1_GetCDNRU";
        public const string actionGSTR1_GetDocIssued = "GSTR1_GetDocIssued";
        public const string actionGSTR1_GetSummary = "GSTR1_GetSummary";
        public const string actionGSTR1_FileDetails = "GSTR1_GetFileDetails";
        public const string actionGSTR1_Submit = "GSTR1_Submit";
        public const string actionGSTR1_FileGSTR1 = "FileGSTR1";
        public const string actionGSTR1_RetSubmit = "GSTR1_RetSubmit";

        //Action Gstr2
        public const string actionGSTR2_SaveGSTR1 = "GSTR2_Save";
        public const string actionGSTR2_GetB2B = "GSTR2_GetB2B";
        public const string actionGSTR2_GetCDN = "GSTR2_GetCDN";
        public const string actionGSTR2_GetB2BUR = "GSTR2_GetB2BUR";
        public const string actionGSTR2_GetCDNUR = "GSTR2_GetCDNUR";
        public const string actionGSTR2_GetHSNSUM = "GSTR2_GetHSNSUM";
        public const string actionGSTR2_GetTrackStatus = "GSTR2_GetTrackStatus";
        public const string actionGSTR2_GetFileDetails = "GSTR2_GetFileDetails";
        public const string actionGSTR2_GetTaxLiablity = "GSTR1_GetTaxLiablity";
        public const string actionGSTR2_GetTxpUnderRC = "GSTR2_GetTxpUnderRC";
        public const string actionGSTR2_GetRetSubmit = "GSTR2_GetRetSubmit";
        public const string actionGSTR2_GetRetSummary = "GSTR2_GetRetSummary";
        public const string actionGSTR2_FileGSTR2 = "FileGSTR2";
        public const string actionGSTR2_RetSubmit = "GSTR2_RetSubmit";
        public const string actionGSTR2_GetNilInvoices = "GSTR2_GetNilInvoices";
        public const string actionGSTR2_GetImpgInvoices = "GSTR2_GetImpgInvoices";
        public const string actionGSTR2_GetImpsInvoices = "GSTR2_GetImpsInvoices";
        public const string actionGSTR2_GetItcRvslInvoices = "GSTR2_GetItcRvslInvoices";
        //Action Gstr2A
        public const string actionGSTR2A_GetB2B = "GSTR2A_GetB2B";
        public const string actionGSTR2A_GetCDN = "GSTR2A_GetCDN";
        public const string actionGSTR2A_GetFileDetails = "GSTR2A_GetFileDetails";

        //Action Gstr3B
        public const string actionGSTR1_SaveGSTR3B = "GSTR3B_Save";
        public const string actionGSTR3B_GetRetSummary = "GSTR3B_GetRetSummary";
        public const string actionGSTR3B_RetSubmit = "GSTR3B_RetSubmit";
        public const string actionGSTR3B_RetOffset = "GSTR3B_RetOffset";
        public const string actionGSTR3B_FileData = "GSTR3B_FileData";
        public const string actionGSTR3B_GetTrackStatus = "GSTR3B_GetTrackStatus";

        //Storage Account
        public static int ReqStorageAccont = Convert.ToInt32(ConfigurationManager.AppSettings["ReqStorageAccCount"]);
        public static int RespStorageAccont = Convert.ToInt32(ConfigurationManager.AppSettings["RespStorageAccCount"]);

        //Download Large File
        public static string TarFilePath = ConfigurationManager.AppSettings["TarFilePath"].ToString();
        public static string TarFileFolder = ConfigurationManager.AppSettings["TarFileFolder"].ToString();
        public static string LargeFileGstnDomain = ConfigurationManager.AppSettings["LargeFileGstnDomain"].ToString();

        //Public API Authentication
        public static string PublicAuthUsername = ConfigurationManager.AppSettings["PublicAuthUsername"].ToString();
        public static string PublicAuthPassword = ConfigurationManager.AppSettings["PublicAuthPassword"].ToString();
        public static string GstCertificate = ConfigurationManager.AppSettings["GstCertificate"].ToString();
        public static string PublicAuthAction = ConfigurationManager.AppSettings["PublicAuthAction"].ToString();
        public static string PublicClientId = ConfigurationManager.AppSettings["PublicClientId"].ToString();
        public static string PublicClientSecret = ConfigurationManager.AppSettings["PublicClientSecret"].ToString();

        public static class SPName
        {
            public const string Gsp_Get_Blob = "Gsp_Get_Blob";
            public const string Gsp_Get_Response_Blob = "Gsp_Get_Response_Blob";
            public const string Gsp_Get_BlobDetail = "Gsp_Get_BlobDetail";
            public const string Gsp_USP_ValidateUser = "Gsp_USP_ValidateUser";
            public const string GSP_Create_RequestTrackLog = "GSP_Create_RequestTrackLog";
            public const string GSP_Create_RequestDehydrationLog = "GSP_Create_RequestDehydrationLog";
            public const string GSP_Create_Exception = "GSP_Create_Exception";
            public const string GSP_Insert_Exception_Log = "GSP_Insert_Exception_Log";
            public const string GSP_INS_BacklogRequest = "GSP_INS_BacklogRequest";
            public const string Get_GSTN_Request_CircuitBreaker_List = "GSP_Get_BacklogRequest";
            public const string Update_GSTN_Request_CircuitBreaker = "GSP_Update_BacklogRequest";
            public const string GSTN_Response_Log = "GSTN_Response_Log";
            public const string Retrieve_Partition_Offset = "Retrieve_Partition_Offset";
            public const string Update_Partition_Offset = "Update_Partition_Offset";
            public const string Retrive_Instance_PartitionId = "Retrive_Instance_PartitionId";
            public const string Update_Release_Partition_Lock = "Update_Release_Partition_Lock";
            public const string Update_Release_Partition_Lock_Instance = "Update_Release_Partition_Lock_Instance";
            public const string Update_BlobPath_By_ReqToken = "UpdateBlobPathByReqToken";
            public const string GSP_Get_GstnResponseByToken = "GSP_Get_GstnResponseByToken";
        }
    }
}
