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
    public class GSTR3BController : ApiController
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

        private static Attrbute objAttribute = new Attrbute();

        [HttpPost]
        [Route("api/GSTR3B/SaveGSTR3B")]
        public ServiceResponse<GstnResponse> SaveGSTR3B(Attrbute objAttr)
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

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                              ? null :
                              req.GetValues("ret_period").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                         ? null :
                         req.GetValues("gstin").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_SaveGSTR3B)
                                   ? null
                                   : Constants.actionGSTR1_SaveGSTR3B;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR3B(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_SaveGSTR3B, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_SaveGSTR3B, (int)Stage.GSTN_Req_API_Error);

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
        [Route("api/GSTR3B/GetRetSum")]
        public ServiceResponse<GstnResponse> GetGSTR3BRetSummary()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR3B_GetRetSummary) ? null : Constants.actionGSTR3B_GetRetSummary;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR3BSummary(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR3B_GetRetSummary, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR3B_GetRetSummary, (int)Stage.GSTN_Req_API_Error);

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

        [HttpPost]
        [Route("api/GSTR3B/RETSUBMIT")]
        public ServiceResponse<GstnResponse> SubmitGSTR3B(Attrbute objAttr)
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

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR3B_RetSubmit) ? null : Constants.actionGSTR3B_RetSubmit;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR3BSubmit(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR3B_RetSubmit, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR3B_RetSubmit, (int)Stage.GSTN_Req_API_Error);

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

        [HttpPost]
        [Route("api/GSTR3B/RETOFFSET")]
        public ServiceResponse<GstnResponse> GSTR3BRetOffset(Attrbute objAttr)
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

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                              ? null :
                              req.GetValues("ret_period").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                         ? null :
                         req.GetValues("gstin").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR3B_RetOffset)
                                   ? null
                                   : Constants.actionGSTR3B_RetOffset;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GSTR3BRetOffset(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR3B_RetOffset, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR3B_RetOffset, (int)Stage.GSTN_Req_API_Error);

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

        [HttpPost]
        [Route("api/GSTR3B/RETFILE")]
        public ServiceResponse<GstnResponse> GSTR3BFileData(Attrbute objAttr)
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

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR3B_FileData) ? null : Constants.actionGSTR3B_FileData;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR3BFileData(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR3B_FileData, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR3B_FileData, (int)Stage.GSTN_Req_API_Error);

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
        [Route("api/GSTR3B/GETRETSTATUS")]
        public ServiceResponse<GstnResponse> GetGSTR3BTrackStatusWithResponse()
        {
            GstnResponse objResponse = new GstnResponse();

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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();
                objAttribute.param_ref_id = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ref_id"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ref_id"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR3B_GetTrackStatus) ? null : Constants.actionGSTR3B_GetTrackStatus;
                #endregion

                objResponse = new GSTR3B_Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR3BTrackStatusWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR3B_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR3B_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);
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
