using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WEP.GSP.Business;
using WEP.GSP.Document;

namespace WEP.GSP.ServiceBus.RestAPI.Controllers
{
    //[Authorize]
    public class ValidationController : ApiController
    {

        private string _authToken;
        private string _clientid;
        private string _statecd;
        private string _username;
        private string _txn;
        private string _clientSecret;
        private string _ipUsr;
        public string _ret_period;
        public string _gstin;

        [HttpPost]
        [Route("api/Authenticate/RequestOtp")]
        public ServiceResponse<Authenticate> RequestOTP(Attrbute objAttr)
        {
            Authenticate objAuthenticate= new Authenticate();

            try
            {
                #region Headers
                var req = Request.Headers;

                _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
                            ? null :
                            req.GetValues("clientid").First();

                _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
                           ? null :
                           req.GetValues("state-cd").First();


                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                #endregion

                objAuthenticate = new UserBusiness(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).RequestOTP(objAttr);


                var respone = new ServiceResponse<Authenticate> { ResponseObject = objAuthenticate, IsError = false };
                return respone;
            }
            catch (Exception ex)
            {
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }

        }

        [HttpPost]
        [Route("api/Authenticate/AuthToken")]
        public ServiceResponse<Authenticate> AuthenticateAuthToken(Attrbute objAttr)
        {
            Authenticate objAuthToken = new Authenticate();

            try
            {
                #region Headers
                var req = Request.Headers;

                _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
                            ? null :
                            req.GetValues("clientid").First();

                _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
                           ? null :
                           req.GetValues("state-cd").First();


                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                #endregion

                objAuthToken = new UserBusiness(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).AuthenticateAuthToken(objAttr);


                var respone = new ServiceResponse<Authenticate> { ResponseObject = objAuthToken, IsError = false };
                return respone;
            }
            catch (Exception ex)
            {
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }

        }

        [HttpPost]
        [Route("api/Authenticate/RefreshToken")]
        public ServiceResponse<Authenticate> RefreshTokenAuthToken(Attrbute objAttr)
        {
            Authenticate objAuthToken = new Authenticate();

            try
            {
                #region Headers
                var req = Request.Headers;

                _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
                            ? null :
                            req.GetValues("clientid").First();

                _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
                           ? null :
                           req.GetValues("state-cd").First();


                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                #endregion

                objAuthToken = new UserBusiness(this._clientid,
                                               this._statecd,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr
                                               ).RefreshTokenAuthToken(objAttr);


                var respone = new ServiceResponse<Authenticate> { ResponseObject = objAuthToken, IsError = false };
                return respone;
            }
            catch (Exception ex)
            {
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }

        }

        [HttpGet]
        [Route("api/GSP/GetResponse")]
        public ServiceResponse<GstnResponse> GetResponseByToken()
        {
            string token = null;

            try
            {

                token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gsptoken"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gsptoken"].ToString();

                GstnResponse objGstnResponse = new GstnResponse();
                UserBusiness objBusiness = new UserBusiness();

                Blob objBlob = new Blob();

                objGstnResponse = objBusiness.GetBlobConnectionByToken(token);


                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objGstnResponse, IsError = false };
                return respone;
            }
            catch (Exception ex)
            {
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }

        }
    }
}
