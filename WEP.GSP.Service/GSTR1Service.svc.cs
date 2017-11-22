using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using WEP.GSP.Business;
using WEP.GSP.Document;
using WEP.GSP.Data;

namespace WEP.GSP.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class GSTR1Service : IGSTR1Service
    {
     
        private readonly string _authToken;

        private string _clientid;
        private string _statecd;
        private string _username;
        private string _txn;      
        private string _clientSecret;
        private string _ipUsr;
        public string _ret_period;
        public string _gstin;

        public GSTR1Service()
        {
            if (WebOperationContext.Current != null)
            {
                IncomingWebRequestContext woc = WebOperationContext.Current.IncomingRequest;

                _clientid = string.IsNullOrEmpty(woc.Headers[Constants.Clientid])
                                                ? null
                                                : woc.Headers[Constants.Clientid];

                _statecd = string.IsNullOrEmpty(woc.Headers[Constants.State_cd])
                                                ? null
                                                : woc.Headers[Constants.State_cd];

                _txn = string.IsNullOrEmpty(woc.Headers[Constants.Txn])
                                               ? null
                                               : woc.Headers[Constants.Txn];

                _clientSecret = string.IsNullOrEmpty(woc.Headers[Constants.Client_secret])
                                               ? null
                                               : woc.Headers[Constants.Client_secret];

                _username = string.IsNullOrEmpty(woc.Headers[Constants.UserName])
                                                ? null
                                                : woc.Headers[Constants.UserName];

                _authToken = string.IsNullOrEmpty(woc.Headers[Constants.Auth_Token])
                                                ? null
                                                : woc.Headers[Constants.Auth_Token];

                _ipUsr = string.IsNullOrEmpty(woc.Headers[Constants.IpUsr])
                                                ? null
                                                : woc.Headers[Constants.IpUsr];

                _ret_period = string.IsNullOrEmpty(woc.Headers[Constants.Ret_period])
                                                ? null
                                                : woc.Headers[Constants.Ret_period];

                _gstin = string.IsNullOrEmpty(woc.Headers[Constants.Gstin])
                                                ? null
                                                : woc.Headers[Constants.Gstin];
            }
        }
        public ServiceResponse<string> GetGSTR1(string  value)
        {
            return new ServiceResponse<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        public ServiceResponse<string> SaveGSTR1(WEP.GSP.Document.Attrbute objAttr)
        {
            try
            {
                var token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken,
                                               this._ret_period,
                                               this._gstin)
                                             .SaveGSTR1(objAttr); 

               var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };
                return respone;
            }
            catch (Exception ex)
            {

                new RequestData().InsertExceptionLog("error while API call - Client:"+ this._clientid+" Username: "+ this._username , ex.Message, ex.Source, (int)WEP.GSP.Document.Stage.WriteAsync_Error);

                var exe = new ExceptionModel { ErrorMessage = ex.Message };

                return new ServiceResponse<string>
                {
                    IsError = true,
                    ExceptionObject = exe
                };

                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objAttr"></param>
        /// <returns></returns>
        //public ServiceResponse<string> SaveGSTR1Sync(WEP.GSP.Document.Attrbute objAttr)
        //{
        //    try
        //    {               
        //        var result = new GSTR1Business(_clientid, _statecd, _username, _txn, _authToken, _clientSecret, _ipUsr,_ret_period,_gstin)
        //                                .SendRequest(objAttr);
        //        var respone  = new ServiceResponse<string> { ResponseObject = result, IsError =false};
        //        return respone;
        //    }
        //    catch(Exception ex)
        //    {
        //        var exe = new ExceptionModel {ErrorMessage = ex.Message};

        //        return new ServiceResponse<string> {
        //            IsError = true,
        //            ExceptionObject = exe
        //        };
        //    }
        //}
    }
}
