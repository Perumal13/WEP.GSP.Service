using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;
using WEP.GSP.GSTN;
using WEP.GSP.Data;
using WEP.GSP.Service.Blob;
using Newtonsoft.Json;

namespace WEP.GSP.Business
{
    public class UserBusiness
    {
        private string _clientid;
        private string _clientSecret;
        private string _ipUsr;
        private string _statecd;
        private string _txn;
        private string _authToken;
        private string _username;
        public UserBusiness()
        { }

        public UserBusiness(string _clientid, string _statecd, string _txn, string _clientSecret, string _ipUsr)
        {
            this._clientid = _clientid;
            this._statecd = _statecd;
            this._txn = _txn;
            this._clientSecret = _clientSecret;
            this._ipUsr = _ipUsr;
        }

        public UserBusiness(string _clientid, string _clientSecret)
        {
            this._clientid = _clientid;
            this._clientSecret = _clientSecret;
        }

        public UserBusiness(string _clientid, string _clientSecret, string _authToken,string _username)
        {
            this._clientid = _clientid;
            this._clientSecret = _clientSecret;
            this._authToken = _authToken;
            this._username = _username;
        }

        public GstnResponse GetBlobConnectionByToken(string token)
        {
            UserData objData = new UserData();
            Blob objBlob = new Blob();
            GstnResponse objResponse = new GstnResponse();

            //string response = "{\"username\":\"WeP.MH.2\",\"reqtoken\":\"6196546c-7e97-4d7b-8f48-9cb5638711e4_636373399124528642\",\"data\":\"8hanCrLzY9xGcGUzDmrgX99Y4gE45o+iOKSVcFVPZV/PXOVNODOdNk9sUwhEF2XVFyt+I+29MzPsD4FwtTmuzGQXtTcQkn2haUCYHn8ELqg=\",\"hmac\":\"i0zctLJMBiHknQfWKbmj9EAiCi8SxKNkCDVZBbLa/Is=\",\"status_cd\":1,\"rek\":\"mEt9TaWPIpiv9Qqf0A/O8fwI2VER+Pz50wZN/gsxx5mVw6sU+AiaO9wRbmJTrWds\",\"apiAction\":\"GSTR1_SaveGSTR1\",\"respBlobId\":0}";
            //var results = JsonConvert.DeserializeObject<GstnResponse>(response);

            objBlob = objData.GetBlobConnectionByToken(token);

            var blobStorageRetry = new BlobWithRetry(objBlob.Connection, Constants.GstnResponseContainer
                                                , token, new Dictionary<string, string>(), objBlob.Id);

            objResponse = blobStorageRetry.DownloadGStnRespBlobRetry();

            return objResponse;
            //return null;

        }

        #region RequestOTP
        /// <summary>
        /// RequestOTP
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public Authenticate RequestOTP(Attrbute objAttr)
        {
            Authenticate objAuthenticate = new Authenticate();

            objAuthenticate = new UserValidation(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).RequestOTP(objAttr);
            return objAuthenticate;
        }        
        #endregion

        #region AuthenticateAuthToken
        /// <summary>
        /// AuthenticateAuthToken
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public Authenticate AuthenticateAuthToken(Attrbute objAttr)
        {
            Authenticate objAuthToken =new Authenticate();
            objAuthToken = new UserValidation(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).AuthenticateAuthToken(objAttr);
            return objAuthToken;
        }
        #endregion

        #region RefreshTokenAuthToken
        /// <summary>
        /// RefreshTokenAuthToken
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public Authenticate RefreshTokenAuthToken(Attrbute objAttr)
        {
            Authenticate objAuthToken = new Authenticate();
            objAuthToken = new UserValidation(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).RefreshAuthToken(objAttr);
            return objAuthToken;
        }
        #endregion

        public Authenticate CommonAuthentication(Attrbute objAttr)
        {
            Authenticate objAuthToken = new Authenticate();

            objAuthToken = new UserValidation(this._clientid,
                                               this._clientSecret
                                               ).CommonAuthentication(objAttr);
            return objAuthToken;
        }

        public GstnResponse SearchTaxpayerOld(string gstin,string action)
        {
            GstnResponse objTaxPayer = new GstnResponse();

            objTaxPayer = new UserValidation(this._clientid,
                                               this._clientSecret,
                                               this._authToken,
                                               this._username
                                               ).SearchTaxpayer(gstin, action);
            return objTaxPayer;

        }

        public GstnResponse SearchTaxpayer(string gstin,string action)
        {
            try
            {
                GstnResponse objResponse = new GstnResponse();

                Authenticate objAuthToken = new Authenticate();
                Attrbute objAttr = new Attrbute();

                EncryptionUtils objEncrypt = new Business.EncryptionUtils();

                byte[] arrKey = objEncrypt.CreateKey();

                string app_key = objEncrypt.EncryptTextWithPublicKey(arrKey);

                string encPassword = objEncrypt.EncryptTextWithPublicKey(Constants.PublicAuthPassword);

                objAttr.username = Constants.PublicAuthUsername;
                objAttr.password = encPassword;
                objAttr.app_key = app_key;
                objAttr.action = Constants.PublicAuthAction;
                string clientid= Constants.PublicClientId;
                string clientsecret = Constants.PublicClientSecret;

                objAuthToken = new UserValidation(clientid,
                                                   clientsecret
                                                   ).CommonAuthentication(objAttr);

                if (objAuthToken.status_cd == "1")
                {
                    objResponse = new UserValidation(clientid,
                                                   clientsecret,
                                                   objAuthToken.auth_token,
                                                   objAttr.username
                                                   ).SearchTaxpayer(gstin, action);
                    return objResponse;
                }                
                else
                {
                    objResponse.error= objAuthToken.error;
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

    }
}
