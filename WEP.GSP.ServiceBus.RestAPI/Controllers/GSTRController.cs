using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WEP.GSP.Business;
using WEP.GSP.Document;
using System.Threading.Tasks;
using System.Web;


namespace WEP.GSP.ServiceBus.RestAPI.Controllers
{
    //[Authorize]
    public class GSTRController : ApiController
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
        [Route("api/GSTR1/SaveGSTR1WithToken")]
        public ServiceResponse<string> SaveGSTR1(Attrbute objAttr)
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
                 
                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_SaveGSTR1) 
                                   ? null 
                                   : Constants.actionGSTR1_SaveGSTR1;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR1(objAttr,true);

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
        [Route("api/GSTR1/GetB2BWithToken")]
        public ServiceResponse<string> GetGSTR1B2B()
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

                objAttribute.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ctin"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2B) ? null : Constants.actionGSTR1_GetB2B;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2B(objAttribute);

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
        [Route("api/GSTR1/GetB2CLWithToken")]
        public ServiceResponse<string> GetGSTR1B2CL()
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

                objAttribute.param_statecd = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["state_cd"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["state_cd"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2CL) ? null : Constants.actionGSTR1_GetB2CL;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2CL(objAttribute);

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
        [Route("api/GSTR1/GetB2CSWithToken")]
        public ServiceResponse<string> GetGSTR1B2CS()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2CS) ? null : Constants.actionGSTR1_GetB2CS;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2CS(objAttribute);

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
        [Route("api/GSTR1/GetNILWithToken")]
        public ServiceResponse<string> GetGSTR1NilInvoice()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetNilInvoices) ? null : Constants.actionGSTR1_GetNilInvoices;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1NilInvoice(objAttribute);

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
        [Route("api/GSTR1/GetTXPWithToken")]
        public ServiceResponse<string> GetGSTR1TxpInvoice()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetTxpInvoices) ? null : Constants.actionGSTR1_GetTxpInvoices;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1TxpInvoice(objAttribute);

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
        [Route("api/GSTR1/GetATWithToken")]
        public ServiceResponse<string> GetGSTR1AT_Invoice()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetAtInvoices) ? null : Constants.actionGSTR1_GetAtInvoices;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1ATInvoice(objAttribute);

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
        [Route("api/GSTR1/GetEXPWithToken")]
        public ServiceResponse<string> GetGSTR1ExpInvoice()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetExpInvoices) ? null : Constants.actionGSTR1_GetExpInvoices;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1ExpInvoice(objAttribute);

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
        [Route("api/GSTR1/GETRETSTATUSWithToken")]
        public ServiceResponse<string> GetGSTR1TrackStatus()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetTrackStatus) ? null : Constants.actionGSTR1_GetTrackStatus;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1TrackStatus(objAttribute);

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
        [Route("api/GSTR1/GetHSNSUMWithToken")]
        public ServiceResponse<string> GetGSTR1HsnSummary()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetHsnSummary) ? null : Constants.actionGSTR1_GetHsnSummary;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1HsnSummary(objAttribute);

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
        [Route("api/GSTR1/GetCDNRWithToken")]
        public ServiceResponse<string> GetGSTR1CDNR()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetCDNR) ? null : Constants.actionGSTR1_GetCDNR;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1CDNR(objAttribute);

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
        [Route("api/GSTR1/GetCDNURWithToken")]
        public ServiceResponse<string> GetGSTR1CDNUR()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetCDNRU) ? null : Constants.actionGSTR1_GetCDNRU;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1CDNRU(objAttribute);

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
        [Route("api/GSTR1/GetDOCISSUEWithToken")]
        public ServiceResponse<string> GetGSTR1DocIssued()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetDocIssued) ? null : Constants.actionGSTR1_GetDocIssued;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1DocIssued(objAttribute);

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
        [Route("api/GSTR1/GetRETSUMWithToken")]
        public ServiceResponse<string> GetGSTR1Summary()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetSummary) ? null : Constants.actionGSTR1_GetSummary;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1Summary(objAttribute);

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
        [Route("api/GSTR1/GetFileDetailsWithToken")]
        public ServiceResponse<string> GetGSTR1FileDetails()
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

                objAttribute.param_token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["token"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["token"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_FileDetails) ? null : Constants.actionGSTR1_FileDetails;
                #endregion

                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1FileDetails(objAttribute);

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


        //WithPayloadAsResponse

        [HttpPost]
        [Route("api/GSTR1/SaveGSTR1")]
        public ServiceResponse<GstnResponse> SaveGSTR1WithResponse(Attrbute objAttr)
        {
            var objResponse = new GstnResponse();
            //string token = null;
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

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_SaveGSTR1)
                                   ? null
                                   : Constants.actionGSTR1_SaveGSTR1;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR1WithResponse(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_SaveGSTR1, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_SaveGSTR1, (int)Stage.GSTN_Req_API_Error);
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
            //return null;
        }

        [HttpGet]
        [Route("api/GSTR1/GetB2B")]
        public ServiceResponse<GstnResponse> GetGSTR1B2BWithResponse()
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

                objAttribute.param_ctin = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ctin"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["ctin"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2B) ? null : Constants.actionGSTR1_GetB2B;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2BWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetB2B, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetB2B, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetB2CL")]
        public ServiceResponse<GstnResponse> GetGSTR1B2CLWithResponse()
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

                objAttribute.param_statecd = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["state_cd"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["state_cd"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2CL) ? null : Constants.actionGSTR1_GetB2CL;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2CLWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetB2CL, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetB2CL, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetB2CS")]
        public ServiceResponse<GstnResponse> GetGSTR1B2CSWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetB2CS) ? null : Constants.actionGSTR1_GetB2CS;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1B2CSWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetB2CS, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetB2CS, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetNIL")]
        public ServiceResponse<GstnResponse> GetGSTR1NilInvoiceWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetNilInvoices) ? null : Constants.actionGSTR1_GetNilInvoices;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1NilInvoiceWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetNilInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetNilInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetTXP")]
        public ServiceResponse<GstnResponse> GetGSTR1TxpInvoiceWithResponse()
        {
            GstnResponse objResponse = new GstnResponse ();

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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetTxpInvoices) ? null : Constants.actionGSTR1_GetTxpInvoices;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1TxpInvoiceWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetTxpInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetTxpInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetAT")]
        public ServiceResponse<GstnResponse> GetGSTR1AT_InvoiceWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetAtInvoices) ? null : Constants.actionGSTR1_GetAtInvoices;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1ATInvoiceWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetAtInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetAtInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetEXP")]
        public ServiceResponse<GstnResponse> GetGSTR1ExpInvoiceWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetExpInvoices) ? null : Constants.actionGSTR1_GetExpInvoices;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1ExpInvoiceWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetExpInvoices, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetExpInvoices, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GETRETSTATUS")]
        public ServiceResponse<GstnResponse> GetGSTR1TrackStatusWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetTrackStatus) ? null : Constants.actionGSTR1_GetTrackStatus;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1TrackStatusWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetTrackStatus, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetHSNSUM")]
        public ServiceResponse<GstnResponse> GetGSTR1HsnSummaryWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetHsnSummary) ? null : Constants.actionGSTR1_GetHsnSummary;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1HsnSummaryWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetHsnSummary, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetHsnSummary, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetCDNR")]
        public ServiceResponse<GstnResponse> GetGSTR1CDNRWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetCDNR) ? null : Constants.actionGSTR1_GetCDNR;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1CDNRWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetCDNR, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetCDNR, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetCDNUR")]
        public ServiceResponse<GstnResponse> GetGSTR1CDNURWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetCDNRU) ? null : Constants.actionGSTR1_GetCDNRU;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1CDNRUWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetCDNRU, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetCDNRU, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetDOCISSUE")]
        public ServiceResponse<GstnResponse> GetGSTR1DocIssuedWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetDocIssued) ? null : Constants.actionGSTR1_GetDocIssued;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1DocIssuedWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetDocIssued, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetDocIssued, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetRETSUM")]
        public ServiceResponse<GstnResponse> GetGSTR1SummaryWithResponse()
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

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_GetSummary) ? null : Constants.actionGSTR1_GetSummary;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1SummaryWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_GetSummary, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_GetSummary, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/GetFileDet")]
        public ServiceResponse<GstnResponse> GetGSTR1FileDetailsWithResponse()
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

                objAttribute.param_token = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["token"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["token"].ToString();

                objAttribute.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_FileDetails) ? null : Constants.actionGSTR1_FileDetails;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1FileDetailsWithResponse(objAttribute);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {

                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_FileDetails, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_FileDetails, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/RetFile")]
        public ServiceResponse<GstnResponse> FileGstr1(Attrbute objAttr)
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

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_FileGSTR1)
                                   ? null
                                   : Constants.actionGSTR1_FileGSTR1;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .FileGstr1(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_FileGSTR1, (int)Stage.GSTN_Req_API_Error);

                var response = new ServiceResponse<GstnResponse>
                {
                    IsError = true,
                    ExceptionObject = new ExceptionModel()
                    {
                        ErrorMessage = cex.Message,
                        Source=cex.Source,
                        KeyParameter = new[] { "ServiceError - GSP Server" }
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_FileGSTR1, (int)Stage.GSTN_Req_API_Error);
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
        [Route("api/GSTR1/RETSUBMIT")]
        public ServiceResponse<GstnResponse> SubmitGSTR1(Attrbute objAttr)
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

                objAttr.apiAction = string.IsNullOrEmpty(Constants.actionGSTR1_RetSubmit) ? null : Constants.actionGSTR1_RetSubmit;
                #endregion

                objResponse = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .GetGSTR1Submit(objAttr);

                var respone = new ServiceResponse<GstnResponse> { ResponseObject = objResponse, IsError = false };
                return respone;
            }
            catch (CustomException cex)
            {
                //Async write to Database
                new ExceptionBusiness().InsertExceptionLog(this._username, cex.Message, Constants.actionGSTR1_RetSubmit, (int)Stage.GSTN_Req_API_Error);

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
                new ExceptionBusiness().InsertExceptionLog(this._username, ex.Message, Constants.actionGSTR1_RetSubmit, (int)Stage.GSTN_Req_API_Error);

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
