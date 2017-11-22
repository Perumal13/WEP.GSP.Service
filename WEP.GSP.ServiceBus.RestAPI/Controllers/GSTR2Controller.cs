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
    public class GSTR2Controller : ApiController
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
        [Route("api/GSTR2/SaveGSTR2WithToken")]
        public ServiceResponse<string> SaveGSTR2(Attrbute objAttribute)
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

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                              ? null :
                              req.GetValues("ret_period").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                         ? null :
                         req.GetValues("gstin").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_SaveGSTR1)
                                   ? null
                                   : Constants.actionGSTR2_SaveGSTR1;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR2(objAttribute);

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
            //return null;
        }

        [HttpGet]
        [Route("api/GSTR2/GetB2BWithToken")]
        public ServiceResponse<string> Gstr2GetB2B()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                //objAttribute.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
                //               ? null
                //               : HttpContext.Current.Request.QueryString["ctin"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetB2B) ? null : Constants.actionGSTR2_GetB2B;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2B2B(objAttribute);

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
        [Route("api/GSTR2/GetCDNWithToken")]
        public ServiceResponse<string> GetGSTR2CDN()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetCDN) ? null : Constants.actionGSTR2_GetCDN;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2CDN(objAttribute);

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
        [Route("api/GSTR2/GetB2BURWithToken")]
        public ServiceResponse<string> GetGSTR2B2BUR()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetB2BUR) ? null : Constants.actionGSTR2_GetB2BUR;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2B2BUR(objAttribute);

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
        [Route("api/GSTR2/GetCDNURWithToken")]
        public ServiceResponse<string> GetGSTR2CDNUR()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetCDNUR) ? null : Constants.actionGSTR2_GetCDNUR;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2CDNUR(objAttribute);

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
        [Route("api/GSTR2/GetHSNSUMWithToken")]
        public ServiceResponse<string> GetGSTR2HsnSummary()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetHSNSUM) ? null : Constants.actionGSTR2_GetHSNSUM;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2HsnSummary(objAttribute);

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
        [Route("api/GSTR2/GetTXLIWithToken")]
        public ServiceResponse<string> GetGSTR2TaxLiability()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTaxLiablity) ? null : Constants.actionGSTR2_GetTaxLiablity;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TaxLiability(objAttribute);

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
        [Route("api/GSTR2/GetTXPWithToken")]
        public ServiceResponse<string> GetGSTR2TaxPaidUnderRC()
        {
            //RC ==> Reverse Charge
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTxpUnderRC) ? null : Constants.actionGSTR2_GetTxpUnderRC;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TaxPaidUnderRC(objAttribute);

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
        [Route("api/GSTR2/GetRetSubmitWithToken")]
        public ServiceResponse<string> GetGSTR2Submit()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetRetSubmit) ? null : Constants.actionGSTR2_GetRetSubmit;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2Submit(objAttribute);

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
        [Route("api/GSTR2/GetRetSummaryWithToken")]
        public ServiceResponse<string> GetGSTR2Summary()
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

                objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["gstin"].ToString();

                objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ret_period"].ToString();

                objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["action"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetRetSummary) ? null : Constants.actionGSTR2_GetRetSummary;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2Summary(objAttribute);

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
        [Route("api/GSTR2/GetTrackStatusWithToken")]
        public ServiceResponse<string> GetGSTR2TrackStatus()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTrackStatus) ? null : Constants.actionGSTR2_GetTrackStatus;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TrackStatus(objAttribute);

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
        [Route("api/GSTR2/GetFileDetWithToken")]
        public ServiceResponse<string> GetGSTR2FileDetails()
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

                objAttribute.param_token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["token"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["token"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetFileDetails) ? null : Constants.actionGSTR2_GetFileDetails;
                #endregion

                token = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2FileDetails(objAttribute);

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




        [HttpPost]
        [Route("api/GSTR2/SaveGSTR2")]
        public ServiceResponse<GstnResponse> SaveGSTR2WithResponse(Attrbute objAttribute)
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_SaveGSTR1)
                                   ? null
                                   : Constants.actionGSTR2_SaveGSTR1;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR2WithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_SaveGSTR1, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_SaveGSTR1, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetB2B")]
        public ServiceResponse<GstnResponse> Gstr2GetB2BWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetB2B) ? null : Constants.actionGSTR2_GetB2B;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2B2BWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                //new ExceptionBusiness().InsertExceptionLog(objResponse.username, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetB2B, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetB2B, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetCDN")]
        public ServiceResponse<GstnResponse> GetGSTR2CDNWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetCDN) ? null : Constants.actionGSTR2_GetCDN;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2CDNWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetCDN, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetCDN, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetB2BUR")]
        public ServiceResponse<GstnResponse> GetGSTR2B2BURWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetB2BUR) ? null : Constants.actionGSTR2_GetB2BUR;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2B2BURWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetB2BUR, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetB2BUR, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetCDNUR")]
        public ServiceResponse<GstnResponse> GetGSTR2CDNURWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetCDNUR) ? null : Constants.actionGSTR2_GetCDNUR;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2CDNURWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetCDNUR, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetCDNUR, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetHSNSUM")]
        public ServiceResponse<GstnResponse> GetGSTR2HsnSummaryWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetHSNSUM) ? null : Constants.actionGSTR2_GetHSNSUM;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2HsnSummaryWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetHSNSUM, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetHSNSUM, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetTXLI")]
        public ServiceResponse<GstnResponse> GetGSTR2TaxLiabilityWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTaxLiablity) ? null : Constants.actionGSTR2_GetTaxLiablity;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TaxLiabilityWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetTaxLiablity, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetTaxLiablity, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetTXP")]
        public ServiceResponse<GstnResponse> GetGSTR2TaxPaidUnderRCWithResponse()
        {
            //RC ==> Reverse Charge
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTxpUnderRC) ? null : Constants.actionGSTR2_GetTxpUnderRC;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TaxPaidUnderRCWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetTxpUnderRC, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetTxpUnderRC, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetRetSummary")]
        public ServiceResponse<GstnResponse> GetGSTR2SummaryWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetRetSummary) ? null : Constants.actionGSTR2_GetRetSummary;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2SummaryWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetRetSummary, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetRetSummary, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetTrackStatus")]
        public ServiceResponse<GstnResponse> GetGSTR2TrackStatusWithResponse()
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

                objAttribute.param_ref_id = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ref_id"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ref_id"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetTrackStatus) ? null : Constants.actionGSTR2_GetTrackStatus;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2TrackStatusWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetFileDet")]
        public ServiceResponse<GstnResponse> GetGSTR2FileDetailsWithResponse()
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

                objAttribute.param_ref_id = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ref_id"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ref_id"].ToString();

                objAttribute.param_token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["token"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["token"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetFileDetails) ? null : Constants.actionGSTR2_GetFileDetails;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2FileDetailsWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetFileDetails, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetFileDetails, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/RetFile")]
        public ServiceResponse<GstnResponse> FileGstr2(Attrbute objAttr)
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

                _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
                              ? null :
                              req.GetValues("ret_period").First();

                _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
                         ? null :
                         req.GetValues("gstin").First();

                _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
                       ? null :
                       req.GetValues("auth-token").First();

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_FileGSTR2)
                                   ? null
                                   : Constants.actionGSTR2_FileGSTR2;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .FileGstr2(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_FileGSTR2, (int)Stage.GSTN_Req_API_Error);

                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = cex.Message,
                        Source = cex.Source,
                        KeyParameter = new[] { "ServiceError - Wep Server" }
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_FileGSTR2, (int)Stage.GSTN_Req_API_Error);
                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = ex.Message,
                        Source = ex.Source,
                        KeyParameter = new[] { "ServiceError - Wep Server" }
                    }
                };
                return response;
            }

        }

        [HttpPost]
        [Route("api/GSTR2/RetSubmit")]
        public ServiceResponse<GstnResponse> SubmitGSTR2WithResponse()
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


                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_RetSubmit) ? null : Constants.actionGSTR2_RetSubmit;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GSTR2SubmitWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_RetSubmit, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_RetSubmit, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetNIL")]
        public ServiceResponse<GstnResponse> GetGSTR2NilInvoiceWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetNilInvoices) ? null : Constants.actionGSTR2_GetNilInvoices;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2NilInvoice(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetNilInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetNilInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetIMPG")]
        public ServiceResponse<GstnResponse> GetImportofGoodsInvoices()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetImpgInvoices) ? null 
                                         : Constants.actionGSTR2_GetImpgInvoices;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2ImportofGoodsInvoices(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetImpgInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetImpgInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetIMPS")]
        public ServiceResponse<GstnResponse> GetImportofServicesBills()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetImpsInvoices) ? null
                                         : Constants.actionGSTR2_GetImpsInvoices;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2ImportofServicesBills(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetImpsInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetImpsInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR2/GetITCRVSL")]
        public ServiceResponse<GstnResponse> GetITCReversalDetails()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetItcRvslInvoices) ? null
                                         : Constants.actionGSTR2_GetItcRvslInvoices;
                #endregion

                objResponse = new GSTR2Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR2ITCReversalDetails(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR2_GetItcRvslInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR2_GetItcRvslInvoices, (int)Stage.GSTN_Req_API_Error);
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

        #region Not Used
        //[HttpGet]
        //[Route("api/GSTR2A/GetB2B")]
        //public ServiceResponse<string> GetGSTR2B2B()
        //{
        //    string token = null;

        //    try
        //    {

        //        #region Headers
        //        var req = Request.Headers;

        //        _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
        //                    ? null :
        //                    req.GetValues("clientid").First();

        //        _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
        //                   ? null :
        //                   req.GetValues("state-cd").First();

        //        _username = string.IsNullOrEmpty(req.GetValues("username").First())
        //                   ? null :
        //                   req.GetValues("username").First();

        //        _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
        //               ? null :
        //               req.GetValues("txn").First();

        //        _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
        //                        ? null :
        //                        req.GetValues("client-secret").First();

        //        _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
        //                 ? null :
        //                 req.GetValues("ip-usr").First();


        //        _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
        //               ? null :
        //               req.GetValues("auth-token").First();

        //        _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
        //               ? null
        //               : req.GetValues("gstin").First();

        //        _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
        //                        ? null
        //                        : req.GetValues("ret_period").First();

        //        objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["gstin"].ToString();

        //        objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["ret_period"].ToString();

        //        objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["action"].ToString();

        //        objAttr.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["ctin"].ToString();

        //        objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetB2B) ? null : Constants.actionGSTR2A_GetB2B;
        //        #endregion

        //        token = new GSTR2Business(this._clientid,
        //                                       this._statecd,
        //                                       this._username,
        //                                       this._txn,
        //                                       this._clientSecret,
        //                                       this._ipUsr,
        //                                       this._authToken,
        //                                       this._ret_period,
        //                                       this._gstin)
        //                                     .GetGSTR2B2B(objAttr);

        //        var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };
        //        return respone;
        //    }
        //    catch (CustomException cex)
        //    {
        //        //Async write to Database
        //        new ExceptionBusiness().InsertExceptionLog(token, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);
        //        var resErr = new ServiceResponse<string> { ResponseObject = cex.Message, IsError = true };
        //        return resErr;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Async write to Database
        //        new ExceptionBusiness().InsertExceptionLog(token, ex.Message, ex.StackTrace, (int)Stage.GSTN_Req_API_Error);
        //        var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
        //        return resErr;
        //    }
        //}


        //[HttpGet]
        //[Route("api/GSTR2A/GetCDN")]
        //public ServiceResponse<string> GetGSTR2CDN()
        //{
        //    string token = null;

        //    try
        //    {

        //        #region Headers
        //        var req = Request.Headers;

        //        _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
        //                    ? null :
        //                    req.GetValues("clientid").First();

        //        _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
        //                   ? null :
        //                   req.GetValues("state-cd").First();

        //        _username = string.IsNullOrEmpty(req.GetValues("username").First())
        //                   ? null :
        //                   req.GetValues("username").First();

        //        _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
        //               ? null :
        //               req.GetValues("txn").First();

        //        _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
        //                        ? null :
        //                        req.GetValues("client-secret").First();

        //        _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
        //                 ? null :
        //                 req.GetValues("ip-usr").First();

        //        _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
        //               ? null :
        //               req.GetValues("auth-token").First();

        //        _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
        //              ? null
        //              : req.GetValues("gstin").First();

        //        _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
        //                        ? null
        //                        : req.GetValues("ret_period").First();

        //        objAttr.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["gstin"].ToString();

        //        objAttr.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["ret_period"].ToString();

        //        objAttr.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["action"].ToString();

        //        objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2A_GetCDN) ? null : Constants.actionGSTR2A_GetCDN;
        //        #endregion

        //        token = new GSTR2Business(this._clientid,
        //                                       this._statecd,
        //                                       this._username,
        //                                       this._txn,
        //                                       this._clientSecret,
        //                                       this._ipUsr,
        //                                       this._authToken,
        //                                       this._ret_period,
        //                                       this._gstin)
        //                                     .GetGSTR2CDN(objAttr);

        //        var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };
        //        return respone;
        //    }
        //    catch (CustomException cex)
        //    {
        //        //Async write to Database
        //        new ExceptionBusiness().InsertExceptionLog(token, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);
        //        var resErr = new ServiceResponse<string> { ResponseObject = cex.Message, IsError = true };
        //        return resErr;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Async write to Database
        //        new ExceptionBusiness().InsertExceptionLog(token, ex.Message, ex.StackTrace, (int)Stage.GSTN_Req_API_Error);
        //        var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
        //        return resErr;
        //    }
        //}

        //[HttpGet]
        //[Route("api/GSTR2/GetRetSubmit")]
        //public ServiceResponse<GstnResponse> GetGSTR2SubmitWithResponse()
        //{
        //    var objResponse = new GstnResponse();

        //    try
        //    {

        //        #region Headers
        //        var req = Request.Headers;

        //        _clientid = string.IsNullOrEmpty(req.GetValues("clientid").First())
        //                    ? null :
        //                    req.GetValues("clientid").First();

        //        _statecd = string.IsNullOrEmpty(req.GetValues("state-cd").First())
        //                   ? null :
        //                   req.GetValues("state-cd").First();

        //        _username = string.IsNullOrEmpty(req.GetValues("username").First())
        //                   ? null :
        //                   req.GetValues("username").First();

        //        _txn = string.IsNullOrEmpty(req.GetValues("txn").First())
        //               ? null :
        //               req.GetValues("txn").First();

        //        _clientSecret = string.IsNullOrEmpty(req.GetValues("client-secret").First())
        //                        ? null :
        //                        req.GetValues("client-secret").First();

        //        _ipUsr = string.IsNullOrEmpty(req.GetValues("ip-usr").First())
        //                 ? null :
        //                 req.GetValues("ip-usr").First();

        //        _authToken = string.IsNullOrEmpty(req.GetValues("auth-token").First())
        //               ? null :
        //               req.GetValues("auth-token").First();

        //        _gstin = string.IsNullOrEmpty(req.GetValues("gstin").First())
        //              ? null
        //              : req.GetValues("gstin").First();

        //        _ret_period = string.IsNullOrEmpty(req.GetValues("ret_period").First())
        //                        ? null
        //                        : req.GetValues("ret_period").First();

        //        objAttribute.param_gstin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["gstin"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["gstin"].ToString();

        //        objAttribute.param_ret_period = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ret_period"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["ret_period"].ToString();

        //        objAttribute.param_action = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["action"].ToString())
        //                       ? null
        //                       : HttpContext.Current.Request.QueryString["action"].ToString();

        //        objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR2_GetRetSubmit) ? null : Constants.actionGSTR2_GetRetSubmit;
        //        #endregion

        //        objResponse = new GSTR2Business(this._clientid,
        //                                       this._statecd,
        //                                       this._username,
        //                                       this._txn,
        //                                       this._clientSecret,
        //                                       this._ipUsr,
        //                                       this._authToken,
        //                                       this._ret_period,
        //                                       this._gstin)
        //                                     .GetGSTR2SubmitWithResponse(objAttribute);

        //        var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
        //        return respone;
        //    }
        //    catch (CustomException cex)
        //    {

        //        new ExceptionBusiness().InsertExceptionLog(objResponse.username, cex.Message, cex.StackTrace, (int)Stage.GSTN_Req_API_Error);

        //        var response = new ServiceResponse<GstnResponse>
        //        {
        //            IsError = true,
        //            ExceptionObject = new ExceptionModel()
        //            {
        //                ErrorMessage = cex.Message,
        //                Source = cex.Source,
        //                KeyParameter = new[] { "ServiceError - GSP Server" }
        //            }
        //        };
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Async write to Database
        //        new ExceptionBusiness().InsertExceptionLog(objResponse.username, ex.Message, ex.Source, (int)Stage.GSTN_Req_API_Error);
        //        var response = new ServiceResponse<GstnResponse>
        //        {
        //            IsError = true,
        //            ExceptionObject = new ExceptionModel()
        //            {
        //                ErrorMessage = ex.Message,
        //                Source = ex.Source,
        //                KeyParameter = new[] { "ServiceError - GSP Server" }
        //            }
        //        };
        //        return response;

        //    }
        //}

        #endregion
    }
}
