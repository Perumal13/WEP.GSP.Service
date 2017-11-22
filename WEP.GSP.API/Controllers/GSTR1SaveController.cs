using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WEP.GSP.Document;

namespace WEP.GSP.API.Controllers
{
    public class GSTR1SaveController : ApiController
    {
        // Post: GSTR1Save     
        string ResData = "", ASPUser = "", AuthToken = "";
        Attributes AttrObj = new Attributes();

        [System.Web.Http.HttpPost]
        public string SaveGSTR1(Attributes objAttr)
        {
            try
            {
                var req = Request.Headers;
                var headers = req.Contains("ASP_USERID");
                var headers1 = req.Contains("AUTH_TOKEN");
                ASPUser = req.GetValues("ASP_USERID").First();
                AuthToken = req.GetValues("AUTH_TOKEN").First();
                if (headers && headers1)
                {
                    if (objAttr == null)
                    {
                        AttrObj.ErrMsg = "Please input proper value";
                        ResData = JsonConvert.SerializeObject(AttrObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        //ResData = "Please input proper value";
                    }
                    else
                    {
                        if (objAttr.data != "" && objAttr.hmac != null && objAttr.action != null)
                        {
                           // fn_GSTR1Save fnGSTR1Save = new fn_GSTR1Save();
                           // ResData = fnGSTR1Save.SendRequest(objAttr, ASPUser, AuthToken);
                        }
                        else
                        {
                            AttrObj.ErrMsg = "Please provide proper value body";
                            ResData = JsonConvert.SerializeObject(AttrObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                            //ResData = "Please input proper value";
                        }
                    }
                }
                else
                {
                    AttrObj.ErrMsg = "Please provide ASP UserId or AuthToken";
                    ResData = JsonConvert.SerializeObject(AttrObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    //ResData = "Please provide ASP UserId or AuthToken";
                }
                //File.WriteAllText(@"E:\GSTR1_New.json", ResData);
                return ResData;
            }
            catch (Exception ex)
            {
                AttrObj.ErrMsg = ex.Message;
                ResData = JsonConvert.SerializeObject(AttrObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return ResData;
            }
        }
    }
}