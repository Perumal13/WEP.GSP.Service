using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using System.Web.Script.Serialization;
using WEP.GSP.EventHub.Queue;
using WEP.GSP.GSTN;
using WEP.GSP.Service.Blob;
using Newtonsoft.Json;
using WEP.GSP.Data;
using System.Threading;

namespace WEP.GSP.Business
{
    public class GSTR2ABusiness
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

        public GSTR2ABusiness() { }


        public GSTR2ABusiness(string clientid, string statecd, string username,
                            string txn, string clientSecret, string ipUsr, string authToken, string ret_period, string gstin)
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

        #region Get B2B

        #region GetGSTR2B2B
        /// <summary>
        /// GetGSTR2B2B
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>        
        public string GetGSTR2AB2B(Attrbute objAttr)
        {
            //generate token
            var token = CommonFunction.GetUniqueToken();

            GSTR2B2B(objAttr, token);

            return token;
        }
        #endregion

        #region GSTR2B2B
        /// <summary>
        /// GSTR2B2B
        /// </summary>
        /// <param name="objAttr"></param>
        private void GSTR2B2B(Attrbute objAttr, string token)
        {
            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();


            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Validation_BEGIN
                                                                            , token
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR2A_GetB2B,
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
                                    param_ctin = objAttr.param_ctin,
                                    param_action = objAttr.param_action,
                                    param_gstin = objAttr.param_gstin,
                                    param_ret_period = objAttr.param_ret_period,
                                    Action = objAttr.action,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttr.apiAction,
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
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));

        }
        #endregion

        #endregion

        #region Get CDN

        #region GetGSTR2CDN
        /// <summary>
        /// GetGSTR2CDN
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public string GetGSTR2ACDN(Attrbute objAttr)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2ACDN(objAttr, token);

            return token;
        }
        #endregion

        #region GSTR2CDN
        /// <summary>
        /// GSTR2CDN
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        private void GSTR2ACDN(Attrbute objAttr, string token)
        {
            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();


            //log to table storage  => P1 started
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Validation_BEGIN
                                                                            , token
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR2A_GetCDN,
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
                                    //param_ctin = objAttr.param_ctin,
                                    param_action = objAttr.param_action,
                                    param_gstin = objAttr.param_gstin,
                                    param_ret_period = objAttr.param_ret_period,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttr.apiAction,
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
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
        }
        #endregion

        #endregion

        //P1 with response as payload

        #region Get B2B With Response
        /// <summary>
        /// GetGSTR2AB2BWithResponse
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2AB2BWithResponse(Attrbute objAttr)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2B2B(objAttr, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;

        }
        #endregion

        #region Get CDN With Response
        /// <summary>
        /// GetGSTR2ACDNWithResponse
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2ACDNWithResponse(Attrbute objAttr)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2ACDN(objAttr, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get File Details With Response

        #region GetGSTR2AFileDetailsWithResponse
        /// <summary>
        /// GetGSTR2AFileDetailsWithResponse
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2AFileDetailsWithResponse(Attrbute objAttr)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2AFileDetails(objAttr, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region GSTR2AFileDetails
        /// <summary>
        /// GSTR2AFileDetails
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        private void GSTR2AFileDetails(Attrbute objAttr, string token)
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
                                    RequestType = RequestType.GSTR2A_Get_FileDetails,
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
                                    param_action = objAttr.param_action,
                                    param_gstin = objAttr.param_gstin,
                                    param_ret_period = objAttr.param_ret_period,
                                    Action = objAttr.action,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttr.apiAction,
                                    BlobFile = token,
                                    param_token = objAttr.param_token
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

        #region ProcessGSTR2A_B2B
        /// <summary>
        /// ProcessGSTR2A_B2B
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR2A_B2B(Request request)
        {
            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN
                                                                            , request.RequestToken
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(request.BlobKey));

            //Sends request GSTN =>GSTR2A for B2B
            request.Response = new GSTR2A().ExecuteGSTR2ARequestWithRetry(request);

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
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS
                                                                            , request.RequestToken
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(request.BlobKey));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }
        #endregion

        #region ProcessGSTR2A_CDN
        /// <summary>
        /// ProcessGSTR2A_CDN
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR2A_CDN(Request request)
        {
            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(request);

            //log to table storage  => P2 started
            Task.Factory.StartNew(() =>
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Response_Invoke_BEGIN
                                                                            , request.RequestToken
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(request.BlobKey));

            //Sends request GSTN =>GSTR2A for B2B
            request.Response = new GSTR2A().ExecuteGSTR2A_CDN_RequestWithRetry(request);

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
            new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , (int)WEP.GSP.Document.Stage.Response_Invoke_SUCCESS
                                                                            , request.RequestToken
                                                                            , Constants.GSTR2AStageTable
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(request.BlobKey));

            var respone = new ServiceResponse<string> { ResponseObject = request.Response, IsError = false };
            return respone;
        }
        #endregion

        #region ProcessGSTR2A_FileDetails
        /// <summary>
        /// ProcessGSTR2A_FileDetails
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2A_FileDetails(Request request)
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
            request.Response = new GSTR2A().ExecuteGSTR2A_FileDetails_RequestWithRetry(request);

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

        //Others
        
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
            throw new AggregateException("Could not process request. Will be re-attempt after some time, Token : " + token, exceptions);
        }

        #endregion

        

    }
}
