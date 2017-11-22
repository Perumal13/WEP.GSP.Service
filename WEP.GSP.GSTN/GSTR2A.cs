using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using WEP.GSP.Data;
using WEP.GSP.Document;
using WEP.Utility;

namespace WEP.GSP.GSTN
{
    public class GSTR2A
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
        public string _ctin;
        public string _action;

        public GSTR2A(string clientId, string stateCd, string userName, string txn
                     , string authToken, string clientSecret, string ipUsr, string ret_period
                     , string gstin, string reqtoken, string ctin, string action)
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
            this._ctin = ctin;
            this._action = action;
        }

        public GSTR2A()
        {
        }

        public static string url = Constants.ApiGSTR2A;

        #region ExecuteGSTR2ARequestWithRetry
        /// <summary>
        /// ExecuteGSTR2ARequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2ARequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2ARequest(request);
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
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                if (remainingTries == 0) //Circit breaker
                {
                    //new Data.RequestData().InsertBacklogRequest(this._reqtoken, this._username, jsonRequest);
                }
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region ExecuteGSTR2ARequest
        /// <summary>
        /// ExecuteGSTR2ARequest
        /// </summary>
        /// <returns></returns>
        private string ExecuteGSTR2ARequest(Request request)
        {
            //string url = Constants.ApiGSTR2A;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2A);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            //query[Constants.querystringCtin] = request.param_ctin;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

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
                        //new RequestData().InsertResponseLog(results);
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

        #region ExecuteGSTR2A_CDN_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2A_CDN_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2A_CDN_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2A_CDN_Request(request);
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
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                if (remainingTries == 0) //Circit breaker
                {
                    //new RequestData().InsertBacklogRequest(this._reqtoken, this._username, jsonRequest);
                }
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }
        #endregion

        #region ExecuteGSTR2A_CDN_Request
        /// <summary>
        /// ExecuteGSTR2A_CDN_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2A_CDN_Request(Request request)
        {
            //string url = Constants.ApiGSTR2A;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2A);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

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
                        //new RequestData().InsertResponseLog(results);
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

        public string ExecuteGSTR2A_FileDetails_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2A_GetFileDetails_Request(request);
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
                    //throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;

            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }

        private string ExecuteGSTR2A_GetFileDetails_Request(Request request)
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

    }
}
