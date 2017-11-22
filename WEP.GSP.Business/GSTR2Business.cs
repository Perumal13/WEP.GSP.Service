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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

namespace WEP.GSP.Business
{
    public class GSTR2Business
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

        public GSTR2Business() { }


        public GSTR2Business(string clientid, string statecd, string username,
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

        #region SaveGstr2

        #region SaveGSTR2
        /// <summary>
        /// SaveGSTR2
        /// </summary>
        /// <param name="attrbute"></param>
        /// <returns></returns>
        public string SaveGSTR2(Attrbute attrbute)
        {
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

            // SaveGSTR1
            GSTR2(attrbute, token, tblConnection, objTableStorage);

            return token;
        }
        #endregion

        #region GSTR2     
        /// <summary>
        /// GSTR2
        /// </summary>
        /// <param name="attrbute"></param>
        /// <param name="token"></param>
        private void GSTR2(Attrbute attrbute, string token, Blob blob, TableStorage objTableStorage)
        {


            //upload  payload to blob storage
            var blobStorage = new BlobStorage(blob.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), blob.Id);

            var blobPath = blobStorage.UploadBlob(attrbute);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.SaveGSTR2,
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

        #region Get B2B

        #region Get GSTR2 B2B
        /// <summary>
        /// GetGSTR2 B2B
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2B2B(Attrbute objAttribute)
        {
            //generate token
            var token = CommonFunction.GetUniqueToken();

            GSTR2B2B(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2B2B
        /// <summary>
        /// GSTR2B2B
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        private void GSTR2B2B(Attrbute objAttr, string token)
        {
            //table storage connection object
            var tblConnection = CommonFunction.GetBlob();


            //log to table storage  => P1 started
            TableStorage objTableStorage = new TableStorage(Constants.PartitionKey, Constants.RowKey
                                                                            , token
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime);

            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Validation_BEGIN));

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR2_GetB2B,
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
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
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
        public string GetGSTR2CDN(Attrbute objAttr)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2CDN(objAttr, token);

            return token;
        }
        #endregion

        #region GSTR2CDN
        /// <summary>
        /// GSTR2CDN
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        private void GSTR2CDN(Attrbute objAttr, string token)
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
                                    RequestType = RequestType.GSTR2_GetCDN,
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
                                                                            , Constants.GSTNStageP1Table
                                                                            , Constants.actualTime)
                                                                            .InsertStageToTableStorage(tblConnection.Keys));
        }
        #endregion

        #endregion

        #region Get B2BUR

        #region GetGSTR2B2BUR
        /// <summary>
        /// GetGSTR2B2BUR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2B2BUR(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2_B2BUR(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2_B2BUR
        /// <summary>
        /// GSTR2_B2BUR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2_B2BUR(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_GetB2BUR,
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

        #region GSTR2 CDNUR

        #region GetGSTR2CDNUR
        /// <summary>
        /// GetGSTR2CDNUR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2CDNUR(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2CDNUR(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2CDNUR
        /// <summary>
        /// GSTR2CDNUR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2CDNUR(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_CDNUR,
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

        #region GSTR2 HsnSummary

        #region GetGSTR2HsnSummary
        /// <summary>
        /// GetGSTR2HsnSummary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2HsnSummary(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2HsnSummary(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2HsnSummary
        /// <summary>
        /// GSTR2HsnSummary 
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2HsnSummary(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_HSNSUM,
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

        #region GSTR2 TaxLiability

        #region GetGSTR2TaxLiability
        /// <summary>
        /// GetGSTR2TaxLiability
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2TaxLiability(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2TaxLiability(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2TaxLiability
        /// <summary>
        /// GSTR2TaxLiability
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2TaxLiability(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_TXLI,
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

        #region GSTR2 TaxPaidUnderRC

        #region GetGSTR2TaxPaidUnderRC
        /// <summary>
        /// GetGSTR2TaxPaidUnderRC
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2TaxPaidUnderRC(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2TaxPaidUnderRC(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2TaxPaidUnderRC
        /// <summary>
        /// GSTR2TaxPaidUnderRC
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2TaxPaidUnderRC(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_TXP,
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

        #region GSTR2 Summary
        #region GetGSTR2Summary
        /// <summary>
        /// GetGSTR2Summary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2Summary(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2Summary(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2Summary
        /// <summary>
        /// GSTR2Summary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2Summary(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_Summary,
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

        #region GSTR2 Submit
        #region GetGSTR2Submit
        /// <summary>
        /// GetGSTR2Submit
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2Submit(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2Submit(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2Submit
        /// <summary>
        /// GSTR2Submit
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2Submit(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_Submit,
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

        #region GSTR2 TrackStatus

        #region GetGSTR2TrackStatus
        /// <summary>
        /// GetGSTR2TrackStatus
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2TrackStatus(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2TrackStatus(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2TrackStatus
        /// <summary>
        /// GSTR2TrackStatus
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2TrackStatus(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_TrackStatus,
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
                                    param_action = objAttribute.param_action,
                                    param_gstin = objAttribute.param_gstin,
                                    param_ret_period = objAttribute.param_ret_period,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttribute.apiAction,
                                    BlobFile = token,
                                    param_ref_id=objAttribute.param_ref_id
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

        #region Get FileDetails

        #region GetGSTR2FileDetails
        /// <summary>
        /// GetGSTR2FileDetails
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR2FileDetails(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR2FileDetails(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR2FileDetails
        /// <summary>
        /// GSTR2FileDetails
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2FileDetails(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_FileDetails,
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
                                    Action = objAttribute.action,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttribute.apiAction,
                                    BlobFile = token,
                                    param_token = objAttribute.param_token
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

        #region File Gstr2

        #region FileGstr2
        /// <summary>
        /// FileGstr2
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse FileGstr2(Attrbute objAttr)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

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

            // Gstr2File
            Gstr2File(objAttr, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region Gstr2File
        /// <summary>
        /// Gstr2File
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        /// <param name="storageAcc"></param>
        /// <param name="objTableStorage"></param>
        private void Gstr2File(Attrbute objAttr, string token, Blob storageAcc, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(storageAcc.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), storageAcc.Id);

            var blobPath = blobStorage.UploadBlob(objAttr);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR2_File,
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

        //Process1 With Response

        #region SaveGSTR2 With Response
        /// <summary>
        /// SaveGSTR2WithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse SaveGSTR2WithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

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

            // SaveGSTR1
            GSTR2(objAttribute, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region Get GSTR2B2B With Response
        /// <summary>
        /// GetGSTR2B2BWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2B2BWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2B2B(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get GSTR2CDN With Response
        /// <summary>
        /// GetGSTR2CDNWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2CDNWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2CDN(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region Get GSTR2 B2BUR With Response
        /// <summary>
        /// GetGSTR2B2BURWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2B2BURWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2_B2BUR(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;

        }
        #endregion

        #region Get GSTR2 CDNUR With Response
        /// <summary>
        /// GetGSTR2CDNURWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2CDNURWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2CDNUR(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region Get TaxLiability With Response
        /// <summary>
        /// GetGSTR2TaxLiabilityWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2TaxLiabilityWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2TaxLiability(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region Get GSTR2 HsnSummary With Response
        /// <summary>
        /// GetGSTR2HsnSummaryWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2HsnSummaryWithResponse(Attrbute objAttribute)
        {

            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2HsnSummary(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region Get GSTR2 TaxPaid Under RC With Response
        /// <summary>
        /// GetGSTR2TaxPaidUnderRCWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2TaxPaidUnderRCWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2TaxPaidUnderRC(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region GSTR2 Submit With Response
        /// <summary>
        /// GSTR2SubmitWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GSTR2SubmitWithResponse(Attrbute objAttribute)
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

            // Submit GSTR2
            GSTR2Submit(objAttribute, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;

        }
        #endregion

        #region GSTR2Submit
        /// <summary>
        /// GSTR2Submit
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        /// <param name="tblConnection"></param>
        /// <param name="objTableStorage"></param>
        private void GSTR2Submit(Attrbute objAttribute, string token, Blob tblConnection, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(tblConnection.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), tblConnection.Id);

            var blobPath = blobStorage.UploadBlob(objAttribute);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR2_RetSubmit,
                                    BlobFile = token,
                                    RequestToken = token,
                                    Blob = tblConnection.Id,
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
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }
        #endregion

        #region Get GSTR2 Summary With Response
        /// <summary>
        /// GetGSTR2SummaryWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2SummaryWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2Summary(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;            
        }
        #endregion

        #region Get GSTR2 TrackStatus With Response
        /// <summary>
        /// GetGSTR2TrackStatusWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2TrackStatusWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2TrackStatus(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
            
        }
        #endregion

        #region Get GSTR2 FileDetails With Response
        /// <summary>
        /// GetGSTR2FileDetailsWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2FileDetailsWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2FileDetails(objAttribute, token);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
            
        }
        #endregion

        #region Get NilInvoice

        #region Get GSTR2 NilInvoice
        /// <summary>
        /// Get NilInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2NilInvoice(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2NilInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }

        #endregion

        #region GSTR2NilInvoice
        /// <summary>
        /// GSTR2NilInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2NilInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_NilInvoice,
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

        #region Get Import of Goods Invoices

        #region GetGSTR2ImportofGoodsInvoices
        /// <summary>
        /// GetGSTR2ImportofGoodsInvoices
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2ImportofGoodsInvoices(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2ImpgInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region GSTR2ImpgInvoice
        /// <summary>
        /// GSTR2ImpgInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2ImpgInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_ImpgInvoice,
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

        #region Get GSTR2 Import of Services Bills
        /// <summary>
        /// GetGSTR2ImportofServicesBills
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2ImportofServicesBills(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2ImpsInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }

        private void GSTR2ImpsInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_ImpsInvoice,
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

        #region Get GSTR2 ITC Reversal Details

        #region GetGSTR2ITCReversalDetails
        /// <summary>
        /// GetGSTR2ITCReversalDetails
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR2ITCReversalDetails(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR2ItcRvslInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region GSTR2ItcRvslInvoice
        /// <summary>
        /// GSTR2ItcRvslInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR2ItcRvslInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR2_Get_ItcRvslInvoice,
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

        //Process - 2        

        #region ProcessGSTR2Save
        /// <summary>
        /// ProcessGSTR2Save
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2Save(Request request)
        {
            #region Download from blob storage

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
            request.Response = new GSTR2(request.Clientid
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
                                        .ExecuteGSTR2RequestWithRetry(attribute);

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

        #region ProcessGSTR2_B2B
        /// <summary>
        /// ProcessGSTR2_B2B
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_B2B(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_B2B_RequestWithRetry(request);

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

        #region ProcessGSTR2_CDN
        /// <summary>
        /// ProcessGSTR2_CDN
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_CDN(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_CDN_RequestWithRetry(request);

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

        #region ProcessGSTR2_B2BUR
        /// <summary>
        /// ProcessGSTR2_B2BUR
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_B2BUR(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_B2BUR_RequestWithRetry(request);

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

        #region ProcessGSTR2_CDNUR
        /// <summary>
        /// ProcessGSTR2_CDNUR
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_CDNUR(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_CDNUR_RequestWithRetry(request);

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

        #region ProcessGSTR2_HsnSummary
        /// <summary>
        /// ProcessGSTR2_HsnSummary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_HsnSummary(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_HsnSummary_RequestWithRetry(request);

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

        #region ProcessGSTR2_TaxLiability
        /// <summary>
        /// ProcessGSTR2_TaxLiability
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_TaxLiability(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_TaxLiability_RequestWithRetry(request);

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

        #region ProcessGSTR2_TaxPaidUnderRC
        /// <summary>
        /// ProcessGSTR2_TaxPaidUnderRC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_TaxPaidUnderRC(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_TaxPaidUnderRC_RequestWithRetry(request);

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

        #region ProcessGSTR2_Summary
        /// <summary>
        /// ProcessGSTR2_Summary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_Summary(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_Summary_RequestWithRetry(request);

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

        #region ProcessGSTR2_Submit
        /// <summary>
        /// ProcessGSTR2_Submit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_Submit(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_Submit_RequestWithRetry(request);

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

        #region ProcessGSTR2_TrackStatus
        /// <summary>
        /// ProcessGSTR2_TrackStatus
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_TrackStatus(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_TrackStatus_RequestWithRetry(request);

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

        #region ProcessGSTR2_FileDetails
        /// <summary>
        /// ProcessGSTR2_FileDetails
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns
        internal ServiceResponse<string> ProcessGSTR2_FileDetails(Request request)
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
            request.Response = new GSTR2().ExecuteGSTR2_FileDetails_RequestWithRetry(request);

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

        #region ProcessGSTR2_File
        /// <summary>
        /// ProcessGSTR2_File
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_File(Request request)
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
            request.Response = new GSTR2(request.Clientid
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
                                        .ExecuteGSTR2FileWithRetry(attribute);

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

        #region ProcessGSTR2_RetSubmit
        /// <summary>
        /// ProcessGSTR2_RetSubmit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_RetSubmit(Request request)
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
            request.Response = new GSTR2(request.Clientid
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
                                        .ExecuteGSTR2_RetSubmit_RequestWithRetry(attribute);

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

        #region Process GSTR2_NilInvoice
        /// <summary>
        /// ProcessGSTR2_NilInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_NilInvoice(Request request)
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

            //Sends request GSTN =>GSTR2 for NilInvoice
            request.Response = new GSTR2().ExecuteGSTR2_NilInvoice_RequestWithRetry(request);

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

        #region ProcessGSTR2_ImpgInvoice
        /// <summary>
        /// ProcessGSTR2_ImpgInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_ImpgInvoice(Request request)
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

            //Sends request GSTN =>GSTR2 for Impg
            request.Response = new GSTR2().ExecuteGSTR2_IMPG_RequestWithRetry(request);

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

        #region ProcessGSTR2_ImpsInvoice
        /// <summary>
        /// ProcessGSTR2_ImpsInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_ImpsInvoice(Request request)
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

            //Sends request GSTN =>GSTR2 for Imps
            request.Response = new GSTR2().ExecuteGSTR2_IMPS_RequestWithRetry(request);

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

        #region ProcessGSTR2_ItcRvslInvoice
        /// <summary>
        /// ProcessGSTR2_ItcRvslInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR2_ItcRvslInvoice(Request request)
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

            //Sends request GSTN =>GSTR2 for ItcRvsl
            request.Response = new GSTR2().ExecuteGSTR2_ItcRvsl_RequestWithRetry(request);

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

        //others

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
            throw new AggregateException("Could not process request. Will be re-attempt after some time, Token : " + token, exceptions);
        }
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
