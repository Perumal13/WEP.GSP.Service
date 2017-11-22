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
    public class GSTR2AController : ApiController
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

        private static Attrbute objAttr = new Attrbute();

        [HttpGet]
        [Route("api/GSTR2A/GetB2BWithToken")]
        public ServiceResponse<string> GetGSTR2AB2B()
        {
            string token = null;

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

                _username = string.IsNullOrEmpty(req.GetValues("username").First())
                           ? null :
                           req.GetValues("username").First();

                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();


                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                       ? null
                       : req.GetValues("gstin").First();

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                                ? null
                                : req.GetValues("ret_period").First();

                objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttr.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ctin"].ToString();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetB2B) ? null : Constants.actionGSTR2A_GetB2B;
                #endregion

                token = new GSTR2ABusiness(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2AB2B(objAttr);

                var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(token, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);
                var resErr = new ServiceResponse<string> { ResponseObject = cex.Message, IsError = true };
                return resErr;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(token, ex.Message, ex.StackTrace, (int)Stage.GSTN_Req_API_Error);
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }
        }

        [HttpGet]
        [Route("api/GSTR2A/GetCDNWithToken")]
        public ServiceResponse<string> GetGSTR2ACDN()
        {
            string token = null;

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

                _username = string.IsNullOrEmpty(req.GetValues("username").First())
                           ? null :
                           req.GetValues("username").First();

                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                      ? null
                      : req.GetValues("gstin").First();

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                                ? null
                                : req.GetValues("ret_period").First();

                objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetCDN) ? null : Constants.actionGSTR2A_GetCDN;
                #endregion

                token = new GSTR2ABusiness(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2ACDN(objAttr);

                var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(token, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);
                var resErr = new ServiceResponse<string> { ResponseObject = cex.Message, IsError = true };
                return resErr;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(token, ex.Message, ex.StackTrace, (int)Stage.GSTN_Req_API_Error);
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }
        }


        [HttpGet]
        [Route("api/GSTR2A/GetB2B")]
        public ServiceResponse<GstnResponse> GetGSTR2AB2BWithResponse()
        {
            var objResponse = new GstnResponse();

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

                _username = string.IsNullOrEmpty(req.GetValues("username").First())
                           ? null :
                           req.GetValues("username").First();

                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();


                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                       ? null
                       : req.GetValues("gstin").First();

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                                ? null
                                : req.GetValues("ret_period").First();

                objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                //objAttr.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
                //               ? null
                //               : HttpContext.Current.Request.QueryString["ctin"].ToString();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetB2B) ? null : Constants.actionGSTR2A_GetB2B;
                #endregion

                objResponse = new GSTR2ABusiness(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2AB2BWithResponse(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2A_GetB2B, (int)Stage.GSTN_Req_API_Error);

                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = cex.Message,
                        Source = cex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2A_GetB2B, (int)Stage.GSTN_Req_API_Error);
                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = ex.Message,
                        Source = ex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;

            }
        }


        [HttpGet]
        [Route("api/GSTR2A/GetCDN")]
        public ServiceResponse<GstnResponse> GetGSTR2ACDNWithResponse()
        {
            var objResponse = new GstnResponse();

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

                _username = string.IsNullOrEmpty(req.GetValues("username").First())
                           ? null :
                           req.GetValues("username").First();

                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                      ? null
                      : req.GetValues("gstin").First();

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                                ? null
                                : req.GetValues("ret_period").First();

                objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetCDN) ? null : Constants.actionGSTR2A_GetCDN;
                #endregion

                objResponse = new GSTR2ABusiness(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2ACDNWithResponse(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2A_GetCDN, (int)Stage.GSTN_Req_API_Error);

                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = cex.Message,
                        Source = cex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2A_GetCDN, (int)Stage.GSTN_Req_API_Error);
                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = ex.Message,
                        Source = ex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;

            }
        }


        [HttpGet]
        [Route("api/GSTR2A/GetFileDet")]
        public ServiceResponse<GstnResponse> GetGSTR2AFileDetailsWithResponse()
        {
            var objResponse = new GstnResponse();

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

                _username = string.IsNullOrEmpty(req.GetValues("username").First())
                           ? null :
                           req.GetValues("username").First();

                _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
                       ? null :
                       req.GetValues("txn").First();

                _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
                                ? null :
                                req.GetValues("client-secret").First();

                _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
                         ? null :
                         req.GetValues("ip-usr").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                      ? null
                      : req.GetValues("gstin").First();

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                                ? null
                                : req.GetValues("ret_period").First();

                objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                //objAttr.param_ref_id = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ref_id"].ToString())
                //               ? null
                //               : HttpContext.Current.Request.QueryString["ref_id"].ToString();

                objAttr.param_token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["token"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["token"].ToString();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetFileDetails) ? null : Constants.actionGSTR2A_GetFileDetails;
                #endregion

                objResponse = new GSTR2ABusiness(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2AFileDetailsWithResponse(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2A_GetFileDetails, (int)Stage.GSTN_Req_API_Error);

                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = cex.Message,
                        Source = cex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2A_GetFileDetails, (int)Stage.GSTN_Req_API_Error);
                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = ex.Message,
                        Source = ex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;

            }
        }

    }
}
