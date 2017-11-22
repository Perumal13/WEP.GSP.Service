using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using System.Web.Script.Serialization;
using System.Threading;
using WEP.GSP.Data;
using System.Net;
using WEP.Utility;
using Newtonsoft.Json;
using System.IO;
using System.Web;

namespace WEP.GSP.GSTN
{
    public class GSTR2
    {
        public static string url = Constants.ApiGSTR2A;
        private string _clientid;
        private string _statecd;
        private string _username;
        private string _txn;
        private string _authToken;
        private string _clientSecret;
        private string _ipUsr;
        private string _retPeriod;
        private string _gstin;
        private string _requestToken;
        private string _apiAction;

        #region GSTR2
        public GSTR2()
        {
        }

        public GSTR2(string clientid, string statecd, 
                     string username, string txn, 
                     string authToken, string clientSecret, string ipUsr, 
                     string retPeriod, string gstin, string requestToken, string apiAction)
        {
            this._clientid = clientid;
            this._statecd = statecd;
            this._username = username;
            this._txn = txn;
            this._authToken = authToken;
            this._clientSecret = clientSecret;
            this._ipUsr = ipUsr;
            this._retPeriod = retPeriod;
            this._gstin = gstin;
            this._requestToken = requestToken;
            this._apiAction = apiAction;
        }
        #endregion


        #region Execute GSTR2 RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2RequestWithRetry
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public string ExecuteGSTR2RequestWithRetry(Attrbute objAttr)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(objAttr);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2Request(jsonAttribute);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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
                //if (remainingTries == 0) //Circit breaker
                //{
                //    new RequestData().InsertBacklogRequest(this._requestToken, this._username, jsonAttribute);
                //}
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
        private string ExecuteGSTR2Request(string jsonAttribute)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR2);

            httpWebRequest.Timeout = 160000;

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
            httpWebRequest.Headers.Add(Constants.Ret_period, this._retPeriod);
            httpWebRequest.Headers.Add(Constants.Gstin, this._gstin);

            var responseStream = httpWebRequest.GetRequestStream();

            using (var streamWriter = new StreamWriter(responseStream))
            {
                streamWriter.Write(jsonAttribute);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statuscode = (int)httpResponse.StatusCode;

                //new ExceptionData().InsertExceptionLog(this._requestToken, null, null, statuscode);

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string response = streamReader.ReadToEnd().ToString();

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.reqtoken = this._requestToken;
                        results.username = this._username;
                        results.apiAction = this._apiAction;

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


        #region Execute GSTR2_B2B RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_B2B_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_B2B_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetB2B_Request(request);
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
                    gstnResp.username = this._username;
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetB2B Request
        /// <summary>
        /// ExecuteGSTR2_GetB2B_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetB2B_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            //query[Constants.querystringCtin] = request.param_ctin;

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


        #region Execute CDN RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_CDN_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_CDN_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetCDN_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetCDN Request
        /// <summary>
        /// ExecuteGSTR2_GetCDN_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetCDN_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            //query[Constants.querystringCtin] = request.param_ctin;

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


        #region Execute B2BUR RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_B2BUR_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_B2BUR_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetB2BUR_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetB2BUR Request
        /// <summary>
        /// ExecuteGSTR2_GetB2BUR_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetB2BUR_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            //query[Constants.querystringCtin] = request.param_ctin;

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


        #region Execute DNUR RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_CDNUR_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_CDNUR_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetB2CDNUR_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetB2CDNUR Request
        /// <summary>
        /// ExecuteGSTR2_GetB2CDNUR_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetB2CDNUR_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute HsnSummary RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_HsnSummary_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_HsnSummary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetHsnSummary_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetHsnSummary Request
        /// <summary>
        /// ExecuteGSTR2_GetHsnSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetHsnSummary_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute TaxLiability RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_TaxLiability_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_TaxLiability_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetTaxLiability_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetTaxLiability Request
        /// <summary>
        /// ExecuteGSTR2_GetTaxLiability_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetTaxLiability_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute TaxPaidUnderRC RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_TaxPaidUnderRC_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_TaxPaidUnderRC_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetTaxPaidUnderRC_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetTaxPaidUnderRC Request
        /// <summary>
        /// ExecuteGSTR2_GetTaxPaidUnderRC_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetTaxPaidUnderRC_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute Summary RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_Summary_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_Summary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetSummary_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetSummary Request
        /// <summary>
        /// ExecuteGSTR2_GetSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetSummary_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute Submit RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_Submit_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_Submit_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetSubmit_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetSubmit Request
        /// <summary>
        /// ExecuteGSTR2_GetSubmit_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetSubmit_Request(Request request)
        {
            //string url = Constants.ApiGSTR1;

            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute TrackStatus RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_TrackStatus_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_TrackStatus_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetTrackStatus_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetTrackStatus Request
        /// <summary>
        /// ExecuteGSTR2_GetTrackStatus_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetTrackStatus_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2_TrackStatus);
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


        #region Execute FileDetails RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_FileDetails_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_FileDetails_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetFileDetails_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GetFileDetails Request
        /// <summary>
        /// ExecuteGSTR2_GetFileDetails_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetFileDetails_Request(Request request)
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


        #region Execute GSTR2File WithRetry
        /// <summary>
        /// ExecuteGSTR2FileWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteGSTR2FileWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2FileRequest(jsonAttribute);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GSTR2File Request
        /// <summary>
        /// ExecuteGSTR2FileRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR2FileRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR2);

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
            httpWebRequest.Headers.Add(Constants.Ret_period, this._retPeriod);
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
                        results.reqtoken = this._requestToken;
                        results.username = this._username;
                        results.apiAction = this._apiAction;

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


        #region Execute RetSubmit RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_Submit_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_RetSubmit_RequestWithRetry(Attrbute request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_RetSubmit_Request(jsonRequest);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute RetSubmit Request
        /// <summary>
        /// ExecuteGSTR2_RetSubmit_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_RetSubmit_Request(string jsonAttribute)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR2);

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
            httpWebRequest.Headers.Add(Constants.Ret_period, this._retPeriod);
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
                        results.reqtoken = this._requestToken;
                        results.username = this._username;
                        results.apiAction = this._apiAction;

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


        #region Execute GSTR2_NilInvoice_Request With Retry
        /// <summary>
        /// ExecuteGSTR2_NilInvoice_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_NilInvoice_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetNilInvoice_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute  GetNil Invoice Request
        /// <summary>
        /// ExecuteGSTR1_GetNilInvoice_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetNilInvoice_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region Execute GSTR2 IMPG Request With Retry
        /// <summary>
        /// ExecuteGSTR2_IMPG_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_IMPG_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetIMPG_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region Execute GSTR2 Get IMPG Request
        /// <summary>
        /// ExecuteGSTR2_GetIMPG_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetIMPG_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region ExecuteGSTR2_IMPS_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_IMPS_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_IMPS_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetIMPS_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region ExecuteGSTR2_GetIMPS_Request
        /// <summary>
        /// ExecuteGSTR2_GetIMPS_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetIMPS_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


        #region ExecuteGSTR2_ItcRvsl_RequestWithRetry
        /// <summary>
        /// ExecuteGSTR2_ItcRvsl_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR2_ItcRvsl_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR2_GetItcRvsl_Request(request);
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
                    gstnResp.reqtoken = this._requestToken;
                    gstnResp.apiAction = this._apiAction;
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

        #region ExecuteGSTR2_GetItcRvsl_Request
        /// <summary>
        /// ExecuteGSTR2_GetItcRvsl_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR2_GetItcRvsl_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR2);
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


    }

}
