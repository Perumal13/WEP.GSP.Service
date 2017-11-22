using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WEP.GSP.Business;
using WEP.GSP.Document;

namespace WEP.GSP.AZFNTrigger.RestAPI.Controllers
{
    public class GSTNReqController : ApiController
    {
        [HttpPost]
        [Route("api/GSTNReq/SendGSTNRequest")]
        public ServiceResponse<string> SendGSTNRequest(Attrbute objAttr)
        {
            string eventhubReqRead = "{\"PartitionKey\":\"PK_SaveGSTR1\",\"RequestType\":1,\"Clientid\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60\",\"Statecd\":\"11\",\"Username\":\"WeP\",\"Txn\":\"returns\",\"ClientSecret\":\"30a28162eb024f6e859a12bbb9c31725\",\"IpUsr\":\"12.8.9l.80\",\"Blob\":2,\"BlobFile\":\"l7xxf1dd5228b4bc4ec28201638d1717a4ae_27_12.8.9l.80_636329771857468386\",\"RequestToken\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636305444387777979\",\"AuthToken\":\"8a227e0ba56042a0acdf98b3477d2c03\",\"CreatedOn\":\"5/1/2017 11:47:52 AM\",\"Response\":null,\"ModifiedOn\":null}";
            objAttr.reqJsonData = eventhubReqRead;

            try
            {
                GSTR1Business objBusiness = new GSTR1Business();

                ServiceResponse<string> response = objBusiness.SendGSTNRequest(objAttr.reqJsonData);

                return response;
            }
            catch (Exception ex)
            {
                var resErr = new ServiceResponse<string> { ResponseObject = ex.Message, IsError = true };
                return resErr;
            }
            return null;
        }

        [HttpGet]
        [Route("api/GSTR/Test")]
        public bool Test()
        {
            return true;
        }
    }
}
