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
    public class UserValidation
    {
        private string _clientid;
        private string _clientSecret;
        private string _ipUsr;
        private string _statecd;
        private string _txn;
        private string _username;
        private string url;
        private string _authToken;

        public UserValidation(string _clientid, string _statecd, string _txn, string _clientSecret, string _ipUsr)
        {
            this._clientid = _clientid;
            this._statecd = _statecd;
            this._txn = _txn;
            this._clientSecret = _clientSecret;
            this._ipUsr = _ipUsr;
        }

        public UserValidation(string _clientid, string _clientSecret)
        {
            this._clientid = _clientid;
            this._clientSecret = _clientSecret;
        }

        public UserValidation(string _clientid, string _clientSecret, string _authToken,string _username)
        {
            this._clientid = _clientid;
            this._clientSecret = _clientSecret;
            this._authToken = _authToken;
            this._username = _username;
        }

        #region RequestOTP
        /// <summary>
        /// RequestOTP
        /// </summary>
        /// <param name="OtpRequest"></param>
        /// <returns></returns>
        public Authenticate RequestOTP(Attrbute OtpRequest)
        {

            Authenticate objAuthenticate = new Authenticate();
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(OtpRequest);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteOtpRequest(jsonAttribute);
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

        #region ExecuteOtpRequest
        /// <summary>
        /// ExecuteOtpRequest
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private Authenticate ExecuteOtpRequest(string jsonAttribute)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.AuthenticateRequest);

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;
            httpWebRequest.Timeout = 10000;
            httpWebRequest.MaximumResponseHeadersLength = 10000;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);


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

                        var results = JsonConvert.DeserializeObject<Authenticate>(response);

                        //response = JsonConvert.SerializeObject(results.status_cd, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return results;
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


        #region RefreshAuthToken
        /// <summary>
        /// RefreshAuthToken
        /// </summary>
        /// <param name="objAuthenticate"></param>
        /// <returns></returns>
        public Authenticate RefreshAuthToken(Attrbute objAuthenticate)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(objAuthenticate);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteRefreshAuthToken(jsonAttribute);
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

        #region ExecuteRefreshAuthToken
        /// <summary>
        /// ExecuteRefreshAuthToken
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private Authenticate ExecuteRefreshAuthToken(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.AuthenticateRequest);

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);


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

                        var results = JsonConvert.DeserializeObject<Authenticate>(response);

                        //response = JsonConvert.SerializeObject(results.status_cd, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return results;
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


        #region AuthenticateAuthToken
        /// <summary>
        /// AuthenticateAuthToken
        /// </summary>
        /// <param name="objAuthenticate"></param>
        /// <returns></returns>
        public Authenticate AuthenticateAuthToken(Attrbute objAuthenticate)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(objAuthenticate);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteAuthTokenRequestWithRetry(jsonAttribute);
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

        #region ExecuteAuthTokenRequestWithRetry
        /// <summary>
        /// ExecuteAuthTokenRequestWithRetry
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private Authenticate ExecuteAuthTokenRequestWithRetry(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.AuthenticateRequest);

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.State_cd, this._statecd);
            httpWebRequest.Headers.Add(Constants.Txn, this._txn);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.IpUsr, this._ipUsr);


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

                        var results = JsonConvert.DeserializeObject<Authenticate>(response);

                        //response = JsonConvert.SerializeObject(results.status_cd, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return results;
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


        #region CommonAuthentication
        /// <summary>
        /// CommonAuthentication
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public Authenticate CommonAuthentication(Attrbute objAttr)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            string jsonAttribute = new JavaScriptSerializer().Serialize(objAttr);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteCommonAuthenticationRequestWithRetry(jsonAttribute);
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

        #region ExecuteCommonAuthenticationWithRetry
        /// <summary>
        /// Execute Common Authentication Request With Retry
        /// </summary>
        /// <param name="jsonAttribute"></param>
        /// <returns></returns>
        private Authenticate ExecuteCommonAuthenticationRequestWithRetry(string jsonAttribute)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.CommonApiAuthenticate);

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.POST;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);

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

                        var results = JsonConvert.DeserializeObject<Authenticate>(response);

                        return results;
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


        public GstnResponse SearchTaxpayer(string gstin,string action)
        {
            int remainingTries = Constants.MaxTrial;
            var exceptions = new List<Exception>();
            //string jsonAttribute = new JavaScriptSerializer().Serialize(objAttr);

            while (remainingTries > 0)
            {
                try
                {
                    return ExecuteSearchTaxpayerRequestWithRetry(gstin,action);
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

        private GstnResponse ExecuteSearchTaxpayerRequestWithRetry(string gstin,string action)
        {

            var uriBuilder = new UriBuilder(Constants.TaxPayerSearch);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[Constants.querystringAction] = action;
            query[Constants.querystringGstin] = gstin;

            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.GET;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, this._clientid);
            httpWebRequest.Headers.Add(Constants.Client_secret, this._clientSecret);
            httpWebRequest.Headers.Add(Constants.Auth_Token, this._authToken);
            httpWebRequest.Headers.Add(Constants.UserName, this._username);
            //var responseStream = httpWebRequest.GetRequestStream();

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

                        return results;

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