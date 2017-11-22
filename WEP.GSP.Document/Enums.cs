using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{

    public enum RequestType
    {
        None = 0,
        SaveGSTR1 = 1,
        GSTR1_GetB2B = 2,
        GSTR1_GetB2CL = 3,
        GSTR1_GetB2CS = 4,
        GSTR1_Get_NilInvoice=5,
        GSTR1_Get_TxpInvoice=6,
        GSTR1_Get_AT_Invoice=7,
        GSTR1_Get_ExpInvoice = 8,
        GSTR1_Get_TrackStatus = 9,
        GSTR1_Get_HsnSummary = 10,
        GSTR1_Get_CDNR = 11,
        GSTR1_Get_CDNRU=12,
        GSTR1_Get_DocIssued = 13,
        GSTR1_Get_Summary = 14,
        GSTR1_Get_FileDetails = 15,
        GSTR1_File=16,
        GSTR1_RetSubmit = 17,

        SaveGSTR2 = 21,
        GSTR2_GetB2B = 22,
        GSTR2_GetCDN = 23,
        GSTR2_GetB2BUR = 24,
        GSTR2_Get_CDNUR = 25,
        GSTR2_Get_HSNSUM=26,
        GSTR2_Get_TXLI = 27,
        GSTR2_Get_TXP = 28,
        GSTR2_Get_Summary=29,
        GSTR2_Get_Submit = 30,
        GSTR2_Get_TrackStatus = 31,
        GSTR2_Get_FileDetails = 32,
        GSTR2_File = 33,
        GSTR2_RetSubmit = 34,
        GSTR2_Get_NilInvoice = 35,
        GSTR2_Get_ImpgInvoice = 36,
        GSTR2_Get_ImpsInvoice = 37,
        GSTR2_Get_ItcRvslInvoice = 38,

        GSTR2A_GetB2B = 41,
        GSTR2A_GetCDN=42,
        GSTR2A_Get_FileDetails = 43,

        SaveGSTR3B =61,
        GSTR3B_Get_Summary = 62,
        GSTR3B_RetSubmit = 63,
        GSTR3B_RetOffset = 64,
        GSTR3B_RetFile=65,
        GSTR3B_Get_TrackStatus=66,
    };

    public enum Stage
    {

        Validation_BEGIN = 1,
        //Validation_END_SUCCESS = 10,
        //Validation_END_ERROR = 9,

        Validation_PayloadSize_Success = 2,
        Validation_PayloadSize_Error = 3,
        //P2_RequestTriggered_AZFN = 4,

        GSTN_Req_API_Error = 5,
        Blob_Upload_Begin = 6,
        Blob_Upload_Completed = 7,
        Blob_Upload_Error = 8,
        Blob_Download_Error =9,

        Request_WRT_BEGIN = 11,
        Request_WRT_SUCCESS = 12,
        Request_WRT_ERROR = 19,

        Response_Invoke_BEGIN = 80,
        Response_Invoke_SUCCESS = 90,
        Response_Invoke_ERROR = 89,

        RequestToken_Recieved = 100,

        WriteAsync_Success = 50,
        WriteAsync_Error = 59,

        Read_Success = 60,
        Read_Error = 69,



        P1_Request_Sent_To_Service = 0,
        P1_Message_sent_To_Master_Event_Hub = 1,

        

        //P2_Validation_Start = 2,
        //P2_Validation_Completed = 3,
        //P3_Start_Reading_From_Master_Event_Hub = 4,
        //P3_Message_Received_From_Master_Event_Hub = 5,
        //P3_Request_Sent_To_GSTR1 = 6,
        //P3_Respnse_Come_From_GSTR1 = 7,
        //P3_GSTN_Audit_Log_Created = 8,



        Req_Read_Event_Hub_Begin = 700,
        Req_Read_Event_Hub_Error = 799,
        Req_Read_Event_Hub_Success =800,

        Write_Resp_EventHub_Begin=850,
        Write_Resp_EventHub_Error = 851,
        Write_Resp_EventHub_Success = 852,

        Response_From_GSTN_Begin = 900,
        Response_From_GSTN_Success = 1000,
        Response_GSTN_Failure = 999,

        None = 0,
        Validation = 1,
        GSTN = 2

    };

    public enum StorageType
    {
        None = 0,
        Blob = 1,
        DocumnetDB = 2,
        FileStorage = 3
    };



}
