using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WEP.GSP.Business;
using WEP.GSP.Document;
using System.Threading.Tasks;

namespace WEP.GSP.RestAPI.Controllers
{
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

        [HttpPost]
        [Route("api/GSTR/SaveGSTR1")]
        public ServiceResponse<string> SaveGSTR1(Attrbute objAttr)
        {
            string token=null;

            try
            {

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


                token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR1(objAttr,false);

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
    }
}
