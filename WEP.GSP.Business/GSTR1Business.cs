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
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.EventHubs;
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
    public class GSTR1Business
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

        #region GSTR1Business
        public GSTR1Business() { }
        
        public GSTR1Business(string clientid, string statecd, string username,
                            string txn, string clientSecret, string ipUsr, string authToken,string ret_period,string gstin)
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
        #endregion

        private static string actualTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture);

        //Process - 1

        #region P1 With Token As Response

        #region SaveGstr1

        #region SaveGSTR1
        /// <summary>
        /// SaveGSTR1
        /// </summary>
        /// <param name="attrbute"></param>
        /// <returns></returns>
        public string SaveGSTR1(WEP.GSP.Document.Attrbute attrbute, bool isServiceBus)
        {
            //generate token
            //var token = CommonFunction.GetUniqueToken(_clientid, _username, _statecd, _ipUsr);

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
            GSTR1(attrbute, token,isServiceBus,tblConnection, objTableStorage);           

            return token;          
        }
        #endregion

        #region GSTR1        
        /// <summary>
        /// GSTR1
        /// </summary>
        /// <param name="attrbute"></param>
        /// <param name="token"></param>
        private void GSTR1(Attrbute attrbute,string token,bool isServiceBus,Blob blob,TableStorage objTableStorage)
        { 
          
             
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(blob.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), blob.Id );
          
            var blobPath =  blobStorage.UploadBlob(attrbute);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request {
                                                RequestType = RequestType.SaveGSTR1,
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
                                                ApiAction= attrbute.apiAction
                                });


            if (isServiceBus)
            {
                //Write to Service Bus
                IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
                masterqueue.WriteAsync(jsonReqst, true);
            }
            else
            {
                //Write to Event Hub Queue
                IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.EventHub();
                masterqueue.WriteAsync(jsonReqst, true);
            }

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(blob.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));

        }
        #endregion

        #endregion

        #region Get B2B Invoices

        #region GetGSTR1B2B
        /// <summary>
        /// GetGSTR1B2B
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1B2B(Attrbute objAttribute)
        {
            //generate token
            var token = CommonFunction.GetUniqueToken();

            GSTR1B2B(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1B2B
        /// <summary>
        /// GSTR1B2B
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        private void GSTR1B2B(Attrbute objAttr, string token)
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
                                    RequestType = RequestType.GSTR1_GetB2B,
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
                                    BlobFile=token
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

        #region GetGSTR1B2CL

        #region GetGSTR1B2CL
        /// <summary>
        /// GetGSTR1 B2CL
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1B2CL(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1B2CL(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1B2CL
        /// <summary>
        /// GSTR1 B2CL
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1B2CL(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_GetB2CL,
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
                                    param_statecd = objAttribute.param_statecd,
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

        #region Get B2CS Invoices 

        #region GetGSTR1_B2CS
        /// <summary>
        /// GetGSTR1_B2CS
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1B2CS(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1B2CS(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1_B2CS
        /// <summary>
        /// GSTR1B2CS
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1B2CS(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_GetB2CS,
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

        #region Get Nil Invoices 

        #region GetGSTR1_NilInvoice
        /// <summary>
        /// GetGSTR1_NilInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1NilInvoice(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1NilInvoice(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1NilInvoice
        /// <summary>
        /// GSTR1NilInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1NilInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_NilInvoice,
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

        #region Get Txp Invoices 

        #region GetGSTR1_TxpInvoice
        /// <summary>
        /// Get GSTR1 Txp Invoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1TxpInvoice(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1TxpInvoice(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1TxpInvoice
        /// <summary>
        /// GSTR1 Txp Invoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1TxpInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_TxpInvoice,
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

        #region Get Exp Invoices 

        #region GetGSTR1_ExpInvoice
        /// <summary>
        /// Get GSTR1 Exp Invoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1ExpInvoice(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1ExpInvoice(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1ExpInvoice
        /// <summary>
        /// GSTR1ExpInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1ExpInvoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_ExpInvoice,
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

        #region Get AT Invoices 

        #region GetGSTR1ATInvoice
        /// <summary>
        /// GetGSTR1ATInvoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1ATInvoice(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1AT_Invoice(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1AT_Invoice
        /// <summary>
        /// GSTR1AT_Invoice
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1AT_Invoice(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_AT_Invoice,
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

        #region Track Status

        #region GetGSTR1TrackStatus
        /// <summary>
        /// GetGSTR1TrackStatus
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1TrackStatus(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1TrackStatus(objAttribute,token);

            return token;
        }
        #endregion

        #region GSTR1TrackStatus
        /// <summary>
        /// GSTR1TrackStatus
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1TrackStatus(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_TrackStatus,
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

        #region Gstr1 Hsn Summary

        #region GetGSTR1HsnSummary
        /// <summary>
        /// GetGSTR1HsnSummary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1HsnSummary(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1HsnSummary(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1HsnSummary
        /// <summary>
        /// GSTR1HsnSummary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1HsnSummary(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_HsnSummary,
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

        #region Gstr1 Cdnr

        #region GetGSTR1CDNR
        /// <summary>
        /// GetGSTR1CDNR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1CDNR(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1CDNR(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1CDNR
        /// <summary>
        /// GSTR1CDNR
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1CDNR(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_CDNR,
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

        #region Gstr1 CDNRU

        #region GetGSTR1CDNRU
        /// <summary>
        /// GetGSTR1CDNRU
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1CDNRU(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            GSTR1CDNRU(objAttribute, token);

            return token;
        }
        #endregion

        #region GSTR1CDNRU
        /// <summary>
        /// GSTR1CDNRU
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void GSTR1CDNRU(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_CDNRU,
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

        #region Gstr1 Doc Issued

        #region GetGSTR1DocIssued
        /// <summary>
        /// GetGSTR1DocIssued
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1DocIssued(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            Gstr1DocIssued(objAttribute, token);

            return token;
        }
        #endregion

        #region Gstr1DocIssued
        /// <summary>
        /// Gstr1DocIssued
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void Gstr1DocIssued(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_DocIssued,
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

        #region Gstr1 Summary

        #region GetGSTR1Summary
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1Summary(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            Gstr1Summary(objAttribute, token);

            return token;
        }
        #endregion

        #region Gstr1Summary
        /// <summary>
        /// Gstr1Summary
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void Gstr1Summary(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_Summary,
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

        #region GSTR1 File Details

        #region GetGSTR1FileDetails
        /// <summary>
        /// GetGSTR1FileDetails
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public string GetGSTR1FileDetails(Attrbute objAttribute)
        {
            var token = CommonFunction.GetUniqueToken();

            Gstr1FileDetails(objAttribute, token);

            return token;
        }
        #endregion

        #region Gstr1FileDetails
        /// <summary>
        /// Gstr1FileDetails
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <param name="token"></param>
        private void Gstr1FileDetails(Attrbute objAttribute, string token)
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
                                    RequestType = RequestType.GSTR1_Get_FileDetails,
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
                                    Action = objAttribute.action,
                                    Blob = tblConnection.Id,
                                    BlobKey = tblConnection.Keys,
                                    ApiAction = objAttribute.apiAction,
                                    BlobFile = token,
                                    param_token=objAttribute.param_token
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

        #endregion

        //With Response

        #region P1 with Payload as Response


        #region SaveGSTR1WithResponse
        /// <summary>
        /// SaveGSTR1WithResponse
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse SaveGSTR1WithResponse(Attrbute objAttr)
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

                // SaveGSTR1
                GSTR1(objAttr, token, true, tblConnection, objTableStorage);

                objResponse = GetRespByTokenWithRetrySmallFile(token);

                return objResponse;
            
        }
        #endregion
        
        #region Get B2B With Response
        /// <summary>
        /// GetGSTR1B2BWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1B2BWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1B2B(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get B2CL With esponse
        /// <summary>
        /// GetGSTR1B2CLWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1B2CLWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1B2CL(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get B2CS With Response
        /// <summary>
        /// GetGSTR1B2CSWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1B2CSWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1B2CS(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get NilInvoice With Response
        /// <summary>
        /// GetGSTR1NilInvoiceWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1NilInvoiceWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1NilInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get TxpInvoice With Response
        /// <summary>
        /// GetGSTR1TxpInvoiceWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1TxpInvoiceWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1TxpInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get ATInvoice With Response
        /// <summary>
        /// GetGSTR1ATInvoiceWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1ATInvoiceWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1AT_Invoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get ExpInvoice With Response
        /// <summary>
        /// GetGSTR1ExpInvoiceWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1ExpInvoiceWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1ExpInvoice(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get Track Status With Response
        /// <summary>
        /// GetGSTR1TrackStatusWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1TrackStatusWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1TrackStatus(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get HsnSummary With Response
        /// <summary>
        /// GetGSTR1HsnSummaryWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1HsnSummaryWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1HsnSummary(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get CDNR With Response
        /// <summary>
        /// GetGSTR1CDNRWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1CDNRWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1CDNR(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get CDNRU With Response
        /// <summary>
        /// GetGSTR1CDNRUWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1CDNRUWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            GSTR1CDNRU(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get DocIssued With Response
        /// <summary>
        /// GetGSTR1DocIssuedWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1DocIssuedWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            Gstr1DocIssued(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get Summary With Response
        /// <summary>
        /// GetGSTR1SummaryWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1SummaryWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            Gstr1Summary(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Get FileDetails With Response
        /// <summary>
        /// GetGSTR1FileDetailsWithResponse
        /// </summary>
        /// <param name="objAttribute"></param>
        /// <returns></returns>
        public GstnResponse GetGSTR1FileDetailsWithResponse(Attrbute objAttribute)
        {
            UserBusiness objBusiness = new UserBusiness();
            GstnResponse objResponse = new GstnResponse();

            var token = CommonFunction.GetUniqueToken();

            Gstr1FileDetails(objAttribute, token);

            objResponse = GetRespByTokenWithRetry(token);

            return objResponse;
        }
        #endregion

        #region Gstr1 File

        #region File Gstr1
        /// <summary>
        ///  FileGstr1
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public GstnResponse FileGstr1(Attrbute attrbute)
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
            int payloadSize = GetSizeOfObject(attrbute);

            if (payloadSize >= Constants.PayloadSize)
            {
                throw new Exception("Error : Payload size exceed 5 MB.");
            }

            // SaveGSTR1
            Gstr1File(attrbute, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }
        #endregion

        #region Gstr1File
        /// <summary>
        /// Gstr1File
        /// </summary>
        /// <param name="objAttr"></param>
        /// <param name="token"></param>
        /// <param name="storageAcc"></param>
        /// <param name="objTableStorage"></param>
        private void Gstr1File(Attrbute objAttr, string token, Blob storageAcc, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(storageAcc.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), storageAcc.Id);

            var blobPath = blobStorage.UploadBlob(objAttr);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR1_File,
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
                                    //PartitionKey = Constants.PK_SaveGSTR1,
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

        public GstnResponse GetGSTR1Submit(Attrbute objAttr)
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

            // Submit GSTR3B
            GSTR1Submit(objAttr, token, tblConnection, objTableStorage);

            objResponse = GetRespByTokenWithRetrySmallFile(token);

            return objResponse;
        }

        private void GSTR1Submit(Attrbute objAttr, string token, Blob tblConnection, TableStorage objTableStorage)
        {
            //upload  payload to blob storage
            var blobStorage = new BlobStorage(tblConnection.Connection, Constants.CotainerPayload, token, new Dictionary<string, string>(), tblConnection.Id);

            var blobPath = blobStorage.UploadBlob(objAttr);

            // prepare message for Event hub
            string jsonReqst = new JavaScriptSerializer()
                                .Serialize(new Request
                                {
                                    RequestType = RequestType.GSTR1_RetSubmit,
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
                                    ApiAction = objAttr.apiAction
                                });

            //Write to Service Bus
            IQueueProcessor masterqueue = new WEP.GSP.EventHub.Queue.ServiceBus();
            masterqueue.WriteAsync(jsonReqst, true);

            //log to table storage  => P1 completed
            Task.Factory.StartNew(() =>
            objTableStorage.InsertStageToTableStorage(tblConnection.Keys, (int)WEP.GSP.Document.Stage.Request_WRT_SUCCESS));
        }


        #endregion

        //Process - 2

        #region Process2

        #region ProcessGSTR1
        /// <summary>
        /// ProcessGSTR1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1(Request request)
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
            request.Response = new GSTR1(request.Clientid
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
                                        .ExecuteGSTR1RequestWithRetry(attribute);

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

        #region ProcessGSTR1_B2B
        /// <summary>
        /// ProcessGSTR1_B2B
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_B2B(Request request)
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
            request.Response = new GSTR1().ExecuteGSTR1_B2B_RequestWithRetry(request);

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

        #region ProcessGSTR1_B2CL
        /// <summary>
        /// ProcessGSTR1 B2CL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_B2CL(Request request)
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

            //Sends request GSTN =>GSTR1 for B2CL
            request.Response = new GSTR1().ExecuteGSTR1_B2CL_RequestWithRetry(request);

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

        #region ProcessGSTR1_B2CS
        /// <summary>
        /// ProcessGSTR1_B2CS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_B2CS(Request request)
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

            //Sends request GSTN =>GSTR1 for B2CS
            request.Response = new GSTR1().ExecuteGSTR1_B2CS_RequestWithRetry(request);

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

        #region ProcessGSTR1_NilInvoice
        /// <summary>
        /// ProcessGSTR1_NilInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_NilInvoice(Request request)
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

            //Sends request GSTN =>GSTR1 for NilInvoice
            request.Response = new GSTR1().ExecuteGSTR1_NilInvoice_RequestWithRetry(request);

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

        #region ProcessGSTR1_ExpInvoice
        /// <summary>
        /// ProcessGSTR1_ExpInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_ExpInvoice(Request request)
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
            request.Response = new GSTR1().ExecuteGSTR1_ExpInvoice_RequestWithRetry(request);

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

        #region ProcessGSTR1_AT_Invoice
        /// <summary>
        /// ProcessGSTR1_AT_Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_AT_Invoice(Request request)
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
            request.Response = new GSTR1().ExecuteGSTR1_AT_Invoice_RequestWithRetry(request);

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

        #region ProcessGSTR1_TxpInvoice
        /// <summary>
        /// Process GSTR1 TxpInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_TxpInvoice(Request request)
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
            request.Response = new GSTR1().ExecuteGSTR1_ExpInvoice_RequestWithRetry(request);

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

        #region ProcessGSTR1_TrackStatus
        /// <summary>
        /// ProcessGSTR1_TrackStatus
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_TrackStatus(Request request)
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
            request.Response = new GSTR1().ExecuteGSTR1_TrackStatus_RequestWithRetry(request);

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

        #region ProcessGSTR1_HsnSummary
        /// <summary>
        /// ProcessGSTR1_HsnSummary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_HsnSummary(Request request)
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

            //Sends request GSTN =>GSTR1 for Hsn Summary
            request.Response = new GSTR1().ExecuteGSTR1_HsnSummary_RequestWithRetry(request);

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

        #region ProcessGSTR1_CdnrSummary
        /// <summary>
        /// ProcessGSTR1_CdnrSummary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_CdnrSummary(Request request)
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

            //Sends request GSTN =>GSTR1 for cdnr Summary
            request.Response = new GSTR1().ExecuteGSTR1_CdnrSummary_RequestWithRetry(request);

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

        #region ProcessGSTR1_CdnruSummary
        /// <summary>
        /// ProcessGSTR1_CdnruSummary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_CdnruSummary(Request request)
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

            //Sends request GSTN =>GSTR1 for cdnru Summary
            request.Response = new GSTR1().ExecuteGSTR1_CdnruSummary_RequestWithRetry(request);

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

        #region ProcessGSTR1_DocIssued
        /// <summary>
        /// ProcessGSTR1_DocIssued
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_DocIssued(Request request)
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

            //Sends request GSTN =>GSTR1 for cdnru Summary
            request.Response = new GSTR1().ExecuteGSTR1_DocIssued_RequestWithRetry(request);

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

        #region ProcessGSTR1_Summary
        /// <summary>
        /// ProcessGSTR1_Summary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse<string> ProcessGSTR1_Summary(Request request)
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

            //Sends request GSTN =>GSTR1 for Summary
            request.Response = new GSTR1().ExecuteGSTR1_Summary_RequestWithRetry(request);

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

        #region ProcessGSTR1_FileDetails
        /// <summary>
        /// ProcessGSTR1_FileDetails
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR1_FileDetails(Request request)
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

            //Sends request GSTN =>GSTR1 for Summary
            request.Response = new GSTR1().ExecuteGSTR1_FileDetails_RequestWithRetry(request);

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

        #region ProcessGSTR1_File
        /// <summary>
        /// ProcessGSTR1_File
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal ServiceResponse<string> ProcessGSTR1_File(Request request)
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
            request.Response = new GSTR1(request.Clientid
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
                                        .ExecuteGSTR1FileWithRetry(attribute);

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

        internal ServiceResponse<string> ProcessGSTR1Submit(Request request)
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
            request.Response = new GSTR1(request.Clientid
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
                                        .ExecuteGSTR1SubmitWithRetry(attribute);

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

        #region Others
        //Others

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

        #region RequsetGSTN
        /// <summary>
        /// RequsetGSTN with data from CB table
        /// </summary>
        /// <param name="reqtoken"></param>
        public void ProcessBacklogRequsetGSTN(BacklogRequest request)
        {
            //var actualTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
            //                                CultureInfo.InvariantCulture);
            var requestData = new RequestData();
            try
            {
                var data = JsonConvert.DeserializeObject<Request>(request.Data);

                requestData.InsertRequest(request.Data, data.RequestToken, data.Response, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Begin, actualTime);

                GSTR1Business objGSTR1Business = new GSTR1Business(data.Clientid
                                                                            , data.Statecd
                                                                            , data.Username
                                                                            , data.Txn
                                                                            , data.ClientSecret
                                                                            , data.IpUsr
                                                                            , data.AuthToken
                                                                            , data.RetPeriod
                                                                            , data.Gstin);


                //Download the Data from blobstorage 
                ServiceResponse<string> response = objGSTR1Business.ProcessGSTR1(data);

                //P4_Response_Come_From_GSTN in db
                requestData.InsertRequest(request.Data, data.RequestToken, response.ResponseObject, (int)WEP.GSP.Document.Stage.Response_From_GSTN_Success, actualTime);

                //new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                //                                        , (int)WEP.GSP.Document.Stage.Response_From_GSTN_Success
                //                                        , data.Clientid
                //                                        , data.Statecd
                //                                        , data.Username
                //                                        , data.Txn
                //                                        , data.ClientSecret
                //                                        , data.IpUsr
                //                                        , data.AuthToken
                //                                        , data.RequestToken
                //                                        , Constants.GSTNStageTable
                //                                        , Constants.currentTime). InsertToTableStorage(string.Empty);

                UpdateResendStatus(request.RequestToken);
            }
            catch (Exception ex)
            {
                //Response_Invoke_ERROR writting in db
                new ExceptionBusiness().InsertExceptionLog(request.RequestToken, ex.Message, ex.StackTrace, (int)WEP.GSP.Document.Stage.Response_Invoke_ERROR);
            }

        }

       
        #endregion

        #region GetPendingRequest
        /// <summary>
        /// Get top 10 log from circuit breaker table
        /// </summary>
        /// <returns></returns>
        public List<BacklogRequest> GetPendingRequest()
        {
            RequestData req = new RequestData();
            List<BacklogRequest> backlogRequest = req.GetPendingRequest();
            return backlogRequest;
        }

        #endregion

        #region UpdateResendStatus
        /// <summary>
        /// UpdateResendStatus
        /// </summary>
        /// <param name="reqtoken"></param>
        public void UpdateResendStatus(string reqtoken)
        {
            RequestData req = new RequestData();
            req.UpdateResendStatus(reqtoken);
        }
        #endregion

        //Not using

        #region SendGSTNRequest
        /// <summary>
        /// SendGSTNRequest
        /// </summary>
        /// <param name="eventhubReqRead"></param>
        /// <returns></returns>
        //public ServiceResponse<string> SendGSTNRequest(string eventhubReqRead)
        //{
        //    var data = JsonConvert.DeserializeObject<Request>(eventhubReqRead);

        //    try
        //    {

        //        new RequestData().InsertRequest(eventhubReqRead, (int)WEP.GSP.Document.Stage.P2_RequestTriggered_AZFN);

        //        ServiceResponse<string> reqResp= InvokeGSTRByServiceBus(data);

        //        new RequestData().InsertRequest(eventhubReqRead, data.RequestToken, data.Response, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Success);

        //        return reqResp;
        //    }
        //    catch(Exception ex)
        //    {
        //        new ExceptionData().InsertExceptionLog(data.RequestToken, ex.Message, ex.Source, (int)WEP.GSP.Document.Stage.P4_Response_Come_From_GSTN_Failure);

        //        var resErr = new ServiceResponse<string> { ResponseObject = data.RequestToken, IsError = false };
        //        return resErr;

        //    }   

        //    return null;
        //}
        #endregion

        #region ProcessGSTR2A_Get
        public ServiceResponse<string> ProcessGSTR2A_Get(Request request)
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

            int remainingTries = Constants.MaxTrialP1;
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
            throw new AggregateException("Could not process request. Will be re-attempt after some time, Token : " +token, exceptions);
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
