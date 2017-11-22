using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using WEP.GSP.Document;
using System.Web.Script.Serialization;
using WEP.GSP.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using WEP.Utility;
using System.Web;

namespace WEP.GSP.GSTN
{
    public class GSTR1
    {

        private string _clientid;
        private string _statecd;
        private string _username;
        private string _txn;
        private string _authToken;
        private string _clientSecret;
        private string _ipUsr;
        public string _ret_period;
        public string _gstin;
        public string _reqtoken;
        public string _api_action;

        public string url;

        #region Constructor
        public GSTR1(string clientId, string stateCd, string userName, string txn
                     , string authToken, string clientSecret, string ipUsr, string ret_period, string gstin, string reqtoken,string api_action)
        {
            this._clientid = clientId;
            this._statecd = stateCd;
            this._username = userName;
            this._txn = txn;
            this._authToken = authToken;
            this._clientSecret = clientSecret;
            this._ipUsr = ipUsr;
            this._ret_period = ret_period;
            this._gstin = gstin;
            this._reqtoken = reqtoken;
            this._api_action = api_action;
        }

        public GSTR1()
        {
        }
        #endregion

        #region ExecuteGSTR1RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1RequestWithRetry
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public string ExecuteGSTR1RequestWithRetry(Attrbute objAttr)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(objAttr);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1Request(jsonAttribute);
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
                    var gstnResp = new GstnResponse();
                    gstnResp.status_cd = 0;
                    gstnResp.username = this._username;
                    gstnResp.reqtoken = this._reqtoken;
                    gstnResp.apiAction = this._api_action;
                    gstnResp.error = new Error
                    {
                        error_cd = ex.Message.Split(':')[1].ToString(),
                        message = ex.Message.ToString()
                    };

                    return JsonConvert.SerializeObject(gstnResp, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }

        #endregion

        #region ExecuteGSTR1Request
        /// <summary>
        /// ExecuteGSTR1Request
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR1Request(string jsonAttribute)
        {
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR1);

            httpWebRequest.Timeout=160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.PUT;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.UserName, this._username);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, this._authToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, this._ret_period);
            httpWebRequest.Headers.Add(Constants.Gstin, this._gstin);

            var responseStream = httpWebRequest.GetRequestStream(); 
            
            using (var streamWriter = new StreamWriter(responseStream))
            {
                streamWriter.Write(jsonAttribute);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statuscode = (int)httpResponse.StatusCode;
              
                if (statuscode == 200)               
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string response = streamReader.ReadToEnd().ToString();

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = this._reqtoken;
                        results.username = this._username;
                        results.apiAction = this._api_action;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }else if (CommonFunction.ShouldRetry(statuscode))  
                {
                    throw new CustomException(statuscode.ToString() ) ;
                } else
                    throw new Exception(statuscode.ToString());
            }  
        }
        #endregion


        #region GSTR1_B2B_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_B2B_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_B2B_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_GetB2B_Request(request);
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
                    //throw ex;
                    var gstnResp = new GstnResponse();
                    gstnResp.status_cd = 0;
                    gstnResp.username = request.Username;
                    gstnResp.reqtoken = request.RequestToken;
                    gstnResp.apiAction = request.ApiAction;
                    gstnResp.error = new Error
                    {
                        error_cd = ex.Message.Split(':')[1].ToString(),
                        message = ex.Message.ToString()
                    };

                    return JsonConvert.SerializeObject(gstnResp, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                //if (remainingTries == 0) //Circit breaker
                //{
                //    new RequestData().InsertBacklogRequest(this._reqtoken, this._username, jsonRequest);
                //}
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }

        #endregion

        #region GSTR1_GetB2B_Request
        /// <summary>
        /// ExecuteGSTR1_GetB2B_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_GetB2B_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            query[Constants.querystringCtin] = request.param_ctin;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region GSTR1_B2CL_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_B2CL_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_B2CL_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_GetB2CL_Request(request);
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
                    var gstnResp = new GstnResponse();
                    gstnResp.status_cd = 0;
                    gstnResp.username = request.Username;
                    gstnResp.reqtoken = request.RequestToken;
                    gstnResp.apiAction = request.ApiAction;
                    gstnResp.error = new Error
                    {
                        error_cd = ex.Message.Split(':')[1].ToString(),
                        message = ex.Message.ToString()
                    };

                    return JsonConvert.SerializeObject(gstnResp, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region GSTR1_GetB2CL_Request
        /// <summary>
        /// ExecuteGSTR1_GetB2B_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_GetB2CL_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            query[Constants.querystringState_cd] = request.param_statecd;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_B2CS_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_B2CS_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_B2CS_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_GetB2CS_Request(request);
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
                    //throw ex;
                    var gstnResp = new GstnResponse();
                    gstnResp.status_cd = 0;
                    gstnResp.username = request.Username;
                    gstnResp.reqtoken = request.RequestToken;
                    gstnResp.apiAction = request.ApiAction;
                    gstnResp.error = new Error
                    {
                        error_cd = ex.Message.Split(':')[1].ToString(),
                        message = ex.Message.ToString()
                    };

                    return JsonConvert.SerializeObject(gstnResp, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                               
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region GetB2CS_Request
        /// <summary>
        /// GetB2CS_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_GetB2CS_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_NilInvoice_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_NilInvoice_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_NilInvoice_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_GetNilInvoice_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }

        #endregion

        #region GetNilInvoice_Request
        /// <summary>
        /// GetNilInvoice_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_GetNilInvoice_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_TxpInvoice_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_TxpInvoice_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_TxpInvoice_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_GetTxpInvoice_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region GetTxpInvoice_Request
        /// <summary>
        /// GetTxpInvoice_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_GetTxpInvoice_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_ExpInvoice_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_ExpInvoice_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_ExpInvoice_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_ExpInvoice_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region GSTR1_ExpInvoice_Request
        /// <summary>
        /// GSTR1_ExpInvoice_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_ExpInvoice_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_AT_Invoice_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_AT_Invoice_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_AT_Invoice_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_AT_Invoice_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_AT_Invoice_Request
        /// <summary>
        /// Get_AT_Invoice_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_AT_Invoice_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_TrackStatus_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_TrackStatus_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_TrackStatus_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_TrackStatus_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_TrackStatus_Request
        /// <summary>
        /// ExecuteGSTR1_Get_TrackStatus_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_TrackStatus_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1_TrackStatus);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            query[Constants.querystringRef_id] = request.param_ref_id;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_HsnSummary_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_HsnSummary_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_HsnSummary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_HsnSummary_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_HsnSummary_Request
        /// <summary>
        /// Get_HsnSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_HsnSummary_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_CdnrSummary_RequestWithRetry
        /// <summary>
        /// Execute GSTR1 CdnrSummary Request With Retry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_CdnrSummary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_CdnrSummary_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_CdnrSummary_Request
        /// <summary>
        /// Get_CdnrSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_CdnrSummary_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;
                        
                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_CdnruSummary_RequestWithRetry
        /// <summary>
        /// Execute GSTR1 CdnruSummary Request With Retry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_CdnruSummary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_CdnruSummary_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_CdnruSummary_Request
        /// <summary>
        /// Get_CdnruSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_CdnruSummary_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }

        }
        #endregion


        #region ExecuteGSTR1_DocIssued_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1 DocIssued Request With Retry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_DocIssued_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_DocIssued_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
               
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_DocIssued_Request
        /// <summary>
        /// Get_DocIssued_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_DocIssued_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region ExecuteGSTR1_Summary_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_Summary_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_Summary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_Summary_Request(request);
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
                    var gstnResp =new GstnResponse();
                    gstnResp.status_cd = 0;
                    gstnResp.username = request.Username;
                    gstnResp.reqtoken = request.RequestToken;
                    gstnResp.apiAction = request.ApiAction;
                    gstnResp.error = new Error
                    {
                        error_cd = ex.Message.Split(':')[1].ToString(),
                        message = ex.Message.ToString()
                    };

                    return JsonConvert.SerializeObject(gstnResp, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Get_Summary_Request
        /// <summary>
        /// Get Summary Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_Summary_Request(Request request)
        {
            //string url = Constants.ApiGSTR1_Common;
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR1);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region Execute FileDetails RequestWithRetry
        /// <summary>
        /// ExecuteGSTR1_FileDetails_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR1_FileDetails_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1_Get_FileDetails_Request(request);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }
            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Execute FileDetails Request
        /// <summary>
        /// ExecuteGSTR1_Get_FileDetails_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR1_Get_FileDetails_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiCommon);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            query[Constants.querystringToken] = request.param_token;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, request.Clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, request.Statecd);
            httpWebRequest.Headers.Add(Constants.UserName, request.Username);
            httpWebRequest.Headers.Add(Constants.Txn, request.Txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, request.AuthToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, request.ClientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, request.IpUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, request.RetPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, request.Gstin);

            string html = string.Empty;
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (Stream stream = httpResponse.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                string response = reader.ReadToEnd();

                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = request.RequestToken;
                        results.username = request.Username;
                        results.apiAction = request.ApiAction;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region Execute GSTR1File WithRetry
        /// <summary>
        /// ExecuteGSTR1FileWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteGSTR1FileWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1FileRequest(jsonAttribute);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Execute GSTR1File Request
        /// <summary>
        /// ExecuteGSTR1FileRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR1FileRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR1);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.UserName, this._username);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, this._authToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, this._ret_period);
            httpWebRequest.Headers.Add(Constants.Gstin, this._gstin);

            var responseStream = httpWebRequest.GetRequestStream();

            using (var streamWriter = new StreamWriter(responseStream))
            {
                streamWriter.Write(jsonAttribute);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string response = streamReader.ReadToEnd().ToString();

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = this._reqtoken;
                        results.username = this._username;
                        results.apiAction = this._api_action;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }
        #endregion


        #region Execute GSTR1 Submit With Retry
        public string ExecuteGSTR1SubmitWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR1SubmitRequest(jsonAttribute);
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
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region Execute GSTR1 Submit Request
        private string ExecuteGSTR1SubmitRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR1);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.UserName, this._username);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Auth_Token, this._authToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);
            httpWebRequest.Headers.Add(Constants.Ret_period, this._ret_period);
            httpWebRequest.Headers.Add(Constants.Gstin, this._gstin);

            var responseStream = httpWebRequest.GetRequestStream();

            using (var streamWriter = new StreamWriter(responseStream))
            {
                streamWriter.Write(jsonAttribute);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string response = streamReader.ReadToEnd().ToString();

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = this._ret_period;
                        results.username = this._username;
                        results.apiAction = this._api_action;

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                else if (CommonFunction.ShouldRetry(statuscode))
                {
                    throw new CustomException(statuscode.ToString());
                }
                else
                    throw new Exception(statuscode.ToString());
            }
        }

        #endregion

    }

}


// Excption Error Handling 504/404/
// Error handling as per MPLS Pipe size
// Exception handling "Network issue" => "400 errors" => 500 errors 
// Gate Keeper / retry
// Circuit Breaker

// TTL => message handling
// Reactive 
// Atleast once and atmost once