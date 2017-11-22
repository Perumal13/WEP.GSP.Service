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
using WEP.GSP.Document;
using WEP.Utility;

namespace WEP.GSP.GSTN
{
    public class GSTR3B
    {
        public static string url = "";
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

        #region GSTR3B

        public GSTR3B()
        {
        }

        public GSTR3B(string clientid, string statecd,
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


        #region Execute SaveGSTR3B RequestWithRetry
        /// <summary>
        /// ExecuteSaveGSTR3BRequestWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteSaveGSTR3BRequestWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteSaveGSTR3BRequest(jsonAttribute);
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

        #region Execute SaveGSTR3B Request
        /// <summary>
        /// ExecuteSaveGSTR3BRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteSaveGSTR3BRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR3B);

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


        #region Execute Summary RequestWithRetry
        /// <summary>
        /// ExecuteGSTR3B_Summary_RequestWithRetry
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string ExecuteGSTR3B_Summary_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR3B_GetSummary_Request(request);
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

        #region Execute GetSummary Request
        /// <summary>
        /// ExecuteGSTR3B_GetSummary_Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ExecuteGSTR3B_GetSummary_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiGSTR3B);
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


        #region Execute Submit RequesttWithRetry
        /// <summary>
        /// ExecuteGSTR3BSubmitWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteGSTR3BSubmitWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR3BSubmitRequest(jsonAttribute);
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

        #region Execute Submit Request
        /// <summary>
        /// ExecuteGSTR3BSubmitRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR3BSubmitRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR3B);

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


        #region Execute RetOffset RequestWithRetry
        /// <summary>
        /// ExecuteGSTR3BRetOffsetWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteGSTR3BRetOffsetWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR3BRetOffsetRequest(jsonAttribute);
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

        #region Execute RetOffset Request
        /// <summary>
        /// ExecuteGSTR3BRetOffsetRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR3BRetOffsetRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR3B);

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

        public string ExecuteGSTR3B_TrackStatus_RequestWithRetry(Request request)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR3B_Get_TrackStatus_Request(request);
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

        private string ExecuteGSTR3B_Get_TrackStatus_Request(Request request)
        {
            var uriBuilder = new UriBuilder(Constants.ApiCommon);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = request.param_action;
            query[Constants.querystringGstin] = request.param_gstin;
            query[Constants.querystringRet_period] = request.param_ret_period;
            query[Constants.querystringRef_id] = request.param_ref_id;

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
        #endregion


        #region Execute FileData RequestWithRetry
        /// <summary>
        /// ExecuteGSTR3BFileDataWithRetry
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string ExecuteGSTR3BFileDataWithRetry(Attrbute attribute)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(attribute);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteGSTR3BFileDataRequest(jsonAttribute);
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

        #region Execute FileData Request
        /// <summary>
        /// ExecuteGSTR3BFileDataRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private string ExecuteGSTR3BFileDataRequest(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR3B);

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

    }
}
