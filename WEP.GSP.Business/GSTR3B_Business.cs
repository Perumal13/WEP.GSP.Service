using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using WEP.GSP.Service.Blob;
using System.Web.Script.Serialization;
using WEP.GSP.EventHub.Queue;
using WEP.GSP.Data;
using WEP.GSP.GSTN;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using WEP.Utility;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace WEP.GSP.Business
{
    public class GSTR3B_Business
    {

        public string _clientid;
        public string _statecd;
        public string _username;
        public string _txn;
        public string _clientSecret;
        public string _ipUsr;
        public string _authToken;
        public string _ret_period;
        public string _gstin;

        public GSTR3B_Business() { }


        public GSTR3B_Business(string clientid, string statecd, string username,
                               string txn, string clientSecret, string ipUsr, string authToken,
                               string ret_period, string gstin)
        {
            this._clientid = clientid;
            this._statecd = statecd;
            this._username = username;
            this._txn = txn;
            this._clientSecret = clientSecret;
            this._ipUsr = ipUsr;
            this._authToken = authToken;
            this._ret_period = ret_period;
            this._gstin = gstin;

        }

      
        //Process 1

        #region SaveGstr3B

        #region SaveGSTR3B
        /// <summary>
        /// SaveGSTR1
        /// </summary>
        /// <param name="attrbute"></param>
        /// <returns></returns>
        public GstnResponse SaveGSTR3B(Attrbute attrbute)
        {
            var objBusiness = new UserBusiness();
            var objResponse = new GstnResponse();

            //generate token
            var token = CommonFunction.GetUniqueToken();

            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();

            WEP.GSP.Service.Blob.TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime);
            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            //paylaod sizecheck
            int payloadSize = GetSizeOfObject(attrbute);

            if (payloadSize >= Constants.PayloadSize)
            {
                throw new Exception("Error : Payload size exceed 5 MB.");
            }

            // SaveGSTR3B
            GSTR3B(attrbute, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }

        #endregion

        #region GSTR3B        
        /// <summary>
        /// GSTR3B
        /// </summary>
        /// <param name="attrbute"></param>
        /// <param name="token"></param>
        private void GSTR3B(Attrbute attrbute, string token,Blob blob, TableStorage objTableStorage)
        {

            //upload  payload to blob storage
            var blobStorage = new BlobStorage(blob.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), blob.Id);

            var blobPath = blobStorage.UploadBlob(attrbute);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.SaveGSTR3B,
                                    BlobFile = token,
                                    RequestToken = token,
                                    Blob = blob.Id,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    //PartitionKey = Constants.PK_SaveGSTR1,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    ApiAction = attrbute.apiAction
                                });

            //Write to Service Bus

            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);
           
            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));

        }
        #endregion

        #endregion

        #region Get GSTR3B Summary

        #region Get GSTR3B Summary
        /// <summary>
        /// GetGSTR3BSummary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR3BSummary(Attrbute objAttribute)
        {
            var objBusiness = new UserBusiness();
            var objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR3BSummary(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region GSTR3BSummary
        /// <summary>
        /// GSTR3B Summary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR3BSummary(Attrbute objAttribute, string token)
        {
            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();


            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Validation_BEGIN
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR3B_Get_Summary,
                                    RequestToken = token,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    param_action = objAttribute.param_action,
                                    param_gstin = objAttribute.param_gstin,
                                    param_ret_period = objAttribute.param_ret_period,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttribute.apiAction,
                                    BlobFile = token
                                });

            //Write to Service Bus
            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
        }

        #endregion

        #endregion

        #region  GSTR3B Submit

        #region GetGSTR3BSubmit
        /// <summary>
        /// GetGSTR3BSubmit
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR3BSubmit(Attrbute objAttribute)
        {
            var objBusiness = new UserBusiness();
            var objResponse = new GstnResponse();

            //generate token
            var token = CommonFunction.GetUniqueToken();

            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();

            WEP.GSP.Service.Blob.TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime);
            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            //paylaod sizecheck
            int payloadSize = GetSizeOfObject(objAttribute);

            if (payloadSize >= Constants.PayloadSize)
            {
                throw new Exception("Error : Payload size exceed 5 MB.");
            }

            // Submit GSTR3B
            GSTR3BSubmit(objAttribute, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region GSTR3BSubmit
        /// <summary>
        /// GSTR3BSubmit
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        /// <param name="storageAcc"></param>
        /// <param name="objTableStorage"></param>
        private void GSTR3BSubmit(Attrbute objAttribute, string token, Blob storageAcc, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(storageAcc.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), storageAcc.Id);

            var blobPath = blobStorage.UploadBlob(objAttribute);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR3B_RetSubmit,
                                    BlobFile = token,
                                    RequestToken = token,
                                    Blob = storageAcc.Id,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    ApiAction = objAttribute.apiAction
                                });

            //Write to Service Bus
            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(storageAcc.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }
        #endregion

        #endregion

        #region GSTR3B RetOffset

        #region GSTR3BRetOffset
        /// <summary>
        /// GSTR3BRetOffset
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse GSTR3BRetOffset(Attrbute objAttr)
        {
            var objBusiness = new UserBusiness();
            var objResponse = new GstnResponse();

            //generate token
            var token = CommonFunction.GetUniqueToken();

            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();

            WEP.GSP.Service.Blob.TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime);
            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            //paylaod sizecheck
            int payloadSize = GetSizeOfObject(objAttr);

            if (payloadSize >= Constants.PayloadSize)
            {
                throw new Exception("Error : Payload size exceed 5 MB.");
            }

            // GSTR3BRetOffset
            RetOffset(objAttr, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }

        internal ServiceResponse<string> ProcessGSTR3BTrackStatus(Request request)
        {
            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(request.BlobKey, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));

            //Sends request GSTN =>GSTR1 for Txp Invoice
            request.Response = new GSTR3B().ExecuteGSTR3B_TrackStatus_RequestWithRetry(request);
            request.ModifiedOn = System.DateTime.UtcNow.ToString();

            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);

            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);

            var blobPath = blobStorageGstn.UploadBlob(results);

            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;

            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(request.BlobKey, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }

        #endregion

        #region RetOffset
        /// <summary>
        /// RetOffset
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        /// <param name="storageAcc"></param>
        /// <param name="objTableStorage"></param>
        private void RetOffset(Attrbute objAttr, string token, Blob storageAcc, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(storageAcc.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), storageAcc.Id);

            var blobPath = blobStorage.UploadBlob(objAttr);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR3B_RetOffset,
                                    BlobFile = token,
                                    RequestToken = token,
                                    Blob = storageAcc.Id,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    ApiAction = objAttr.apiAction
                                });

            //Write to Service Bus

            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(storageAcc.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }
        #endregion

        #endregion

        #region GSTR3B FileData

        #region GetGSTR3BFileData
        /// <summary>
        /// GetGSTR3BFileData
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR3BFileData(Attrbute objAttr)
        {
            var objBusiness = new UserBusiness();
            var objResponse = new GstnResponse();

            //generate token
            var token = CommonFunction.GetUniqueToken();

            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();

            WEP.GSP.Service.Blob.TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime);
            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            //paylaod sizecheck
            int payloadSize = GetSizeOfObject(objAttr);

            if (payloadSize >= Constants.PayloadSize)
            {
                throw new Exception("Error : Payload size exceed 5 MB.");
            }

            // GSTR3BRetOffset
            RetFileData(objAttr, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region RetFileData
        /// <summary>
        /// RetFileData
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        /// <param name="storageAcc"></param>
        /// <param name="objTableStorage"></param>
        private void RetFileData(Attrbute objAttr, string token, Blob storageAcc, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(storageAcc.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), storageAcc.Id);

            var blobPath = blobStorage.UploadBlob(objAttr);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR3B_RetFile,
                                    BlobFile = token,
                                    RequestToken = token,
                                    Blob = storageAcc.Id,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    ApiAction = objAttr.apiAction
                                });

            //Write to Service Bus

            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(storageAcc.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }
        #endregion
        #endregion

        #region Get GSTR3B TrackStatus With Response
        #region GetGSTR3BTrackStatusWithResponse
        /// <summary>
        /// GetGSTR3BTrackStatusWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR3BTrackStatusWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR3BTrackStatus(objAttribute, token);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region GSTR3BTrackStatus
        /// <summary>
        /// GSTR3BTrackStatus
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR3BTrackStatus(Attrbute objAttribute, string token)
        {
            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();

            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , token
                                       , Constants.GSTNStageP1Table
                                       , Constants.actualTime);

            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR3B_Get_TrackStatus,
                                    RequestToken = token,
                                    Clientid = _clientid,
                                    Statecd = _statecd,
                                    Username = _username,
                                    Txn = _txn,
                                    ClientSecret = _clientSecret,
                                    IpUsr = _ipUsr,
                                    AuthToken = _authToken,
                                    CreatedOn = System.DateTime.UtcNow,
                                    //PartitionKey = Constants.PK_SaveGSTR1,
                                    RetPeriod = this._ret_period,
                                    Gstin = this._gstin,
                                    param_action = objAttribute.param_action,
                                    param_gstin = objAttribute.param_gstin,
                                    param_ret_period = objAttribute.param_ret_period,
                                    param_ref_id = objAttribute.param_ref_id,
                                    Action = objAttribute.action,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttribute.apiAction,
                                    BlobFile = token
                                });

            //Write to Service Bus
            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }
        #endregion
        #endregion

        //Process 2 

        #region Process GSTR3B Save
        /// <summary>
        /// ProcessGSTR3BSave
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR3BSave(Request request)
        {
            #region Download from blob storage
            // ConfigData configData = new ConfigData();
            var blob = new WEP.GSP.CacheManager.CacheManager().GetBlob(request.Blob);

            var blobStorageRetry = new BlobWithRetry(blob.Connection, Constants.CotainerPayload
                                                , request.BlobFile, new Dictionary<string, string>(), request.Blob);

            Attrbute attribute = blobStorageRetry.DownloadBlobRetry();
            #endregion

            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));


            //Sends request GSTN
            request.Response = new GSTR3B(request.Clientid
                                        , request.Statecd
                                        , request.Username
                                        , request.Txn
                                        , request.AuthToken
                                        , request.ClientSecret
                                        , request.IpUsr
                                        , request.RetPeriod
                                        , request.Gstin
                                        , request.RequestToken
                                        , request.ApiAction)
                                        .ExecuteSaveGSTR3BRequestWithRetry(attribute);

            request.ModifiedOn = System.DateTime.UtcNow.ToString();

            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);


            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);


            var blobPath = blobStorageGstn.UploadBlob(results);


            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;
            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //Pipeline Handler

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }        
        #endregion

        #region Process GSTR3B Summary
        /// <summary>
        /// ProcessGSTR3BSummary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR3BSummary(Request request)
        {
            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(request.BlobKey, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));

            //Sends request GSTN =>GSTR2A for B2B
            request.Response = new GSTR3B().ExecuteGSTR3B_Summary_RequestWithRetry(request);

            request.ModifiedOn = System.DateTime.UtcNow.ToString();


            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);

            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);


            var blobPath = blobStorageGstn.UploadBlob(results);

            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;

            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(request.BlobKey, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }

       
        #endregion

        #region Process GSTR3BSubmit
        /// <summary>
        /// ProcessGSTR3BSubmit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR3BSubmit(Request request)
        {
            #region Download from blob storage
            // ConfigData configData = new ConfigData();
            var blob = new WEP.GSP.CacheManager.CacheManager().GetBlob(request.Blob);

            var blobStorageRetry = new BlobWithRetry(blob.Connection, Constants.CotainerPayload
                                                , request.BlobFile, new Dictionary<string, string>(), request.Blob);

            Attrbute attribute = blobStorageRetry.DownloadBlobRetry();
            #endregion

            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));


            //Sends request GSTN
            request.Response = new GSTR3B(request.Clientid
                                        , request.Statecd
                                        , request.Username
                                        , request.Txn
                                        , request.AuthToken
                                        , request.ClientSecret
                                        , request.IpUsr
                                        , request.RetPeriod
                                        , request.Gstin
                                        , request.RequestToken
                                        , request.ApiAction)
                                        .ExecuteGSTR3BSubmitWithRetry(attribute);

            request.ModifiedOn = System.DateTime.UtcNow.ToString();

            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);


            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);


            var blobPath = blobStorageGstn.UploadBlob(results);


            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;
            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //Pipeline Handler

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }
        #endregion

        #region ProcessGSTR3BRetOffset
        /// <summary>
        /// ProcessGSTR3BRetOffset
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR3BRetOffset(Request request)
        {
            #region Download from blob storage
            // ConfigData configData = new ConfigData();
            var blob = new WEP.GSP.CacheManager.CacheManager().GetBlob(request.Blob);

            var blobStorageRetry = new BlobWithRetry(blob.Connection, Constants.CotainerPayload
                                                , request.BlobFile, new Dictionary<string, string>(), request.Blob);

            Attrbute attribute = blobStorageRetry.DownloadBlobRetry();
            #endregion

            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));


            //Sends request GSTN
            request.Response = new GSTR3B(request.Clientid
                                        , request.Statecd
                                        , request.Username
                                        , request.Txn
                                        , request.AuthToken
                                        , request.ClientSecret
                                        , request.IpUsr
                                        , request.RetPeriod
                                        , request.Gstin
                                        , request.RequestToken
                                        , request.ApiAction)
                                        .ExecuteGSTR3BRetOffsetWithRetry(attribute);

            request.ModifiedOn = System.DateTime.UtcNow.ToString();

            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);


            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);


            var blobPath = blobStorageGstn.UploadBlob(results);


            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;
            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //Pipeline Handler

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }
        #endregion

        #region ProcessGSTR3BFileData
        /// <summary>
        /// ProcessGSTR3BFileData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR3BFileData(Request request)
        {
            #region Download from blob storage
            // ConfigData configData = new ConfigData();
            var blob = new WEP.GSP.CacheManager.CacheManager().GetBlob(request.Blob);

            var blobStorageRetry = new BlobWithRetry(blob.Connection, Constants.CotainerPayload
                                                , request.BlobFile, new Dictionary<string, string>(), request.Blob);

            Attrbute attribute = blobStorageRetry.DownloadBlobRetry();
            #endregion

            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                       , request.RequestToken
                                       , Constants.GSTNStageP2Table
                                       , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN));


            //Sends request GSTN
            request.Response = new GSTR3B(request.Clientid
                                        , request.Statecd
                                        , request.Username
                                        , request.Txn
                                        , request.AuthToken
                                        , request.ClientSecret
                                        , request.IpUsr
                                        , request.RetPeriod
                                        , request.Gstin
                                        , request.RequestToken
                                        , request.ApiAction)
                                        .ExecuteGSTR3BFileDataWithRetry(attribute);

            request.ModifiedOn = System.DateTime.UtcNow.ToString();

            //Upload response to blob
            var gstnStorageRespBlob = CommonFunction.GetGstnRespBlob();

            var blobStorageGstn = new BlobStorage(gstnStorageRespBlob.Connection, Constants.GstnResponseContainer
                                                , request.BlobFile, new Dictionary<string, string>(), gstnStorageRespBlob.Id);


            var results = JsonConvert.DeserializeObject<GstnResponse>(request.Response);


            var blobPath = blobStorageGstn.UploadBlob(results);


            results.reqtoken = request.RequestToken;
            results.username = request.Username;
            results.apiAction = request.ApiAction;
            results.blobUrl = blobPath;
            results.respBlobId = gstnStorageRespBlob.Id;

            //save response blob url to database table
            new RequestData().InsertResponseLog(results);

            //Pipeline Handler

            //log to table storage  => P2 end
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }
        #endregion

        //Others

        #region Others

        #region GetSizeOfObject
        /// <summary>
        /// check size of payload
        /// </summary>
        /// <param name="TestObject"></param>
        /// <returns></returns>
        private int GetSizeOfObject(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }

        #endregion

        #region GetRespByTokenWithRetry
        /// <summary>
        /// GetRespByTokenWithRetry
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public GstnResponse GetRespByTokenWithRetry(string token)
        {
            GstnResponse objResponse = new GstnResponse();
            UserBusiness objBusiness = new UserBusiness();

            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(token);

            int sleeptime = 1;

            while (remainingTries > 0)
            {
                try
                {
                    return objBusiness.GetBlobConnectionByToken(token);
                }
                catch (TimeoutException e)
                {
                    exceptions.Add(e);
                }
                catch (CustomException cex)
                {
                    exceptions.Add(cex);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialmsP1 * sleeptime);

                remainingTries--;
                sleeptime++;

            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time, Token :"+ token , exceptions);
        }

        #endregion

        #endregion

        #region GetRespByTokenWithRetrySmallFile
        /// <summary>
        /// GetRespByTokenWithRetry
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public GstnResponse GetRespByTokenWithRetrySmallFile(string token)
        {
            GstnResponse objResponse = new GstnResponse();
            UserBusiness objBusiness = new UserBusiness();

            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(token);

            int sleeptime = 1;

            while (remainingTries > 0)
            {
                try
                {
                    return objBusiness.GetBlobConnectionByToken(token);
                }
                catch (TimeoutException e)
                {
                    exceptions.Add(e);
                }
                catch (CustomException cex)
                {
                    exceptions.Add(cex);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms * sleeptime);

                remainingTries--;
                sleeptime++;

            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time, Token : " + token, exceptions);
        }

        #endregion

    }
}
