using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using WepAuthPPIndia.Models;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RestSharp;
using WEP.GSP.Document;

namespace WepAuthPPIndia.Controllers
{
    public class OAuthController : ApiController
    {
        Attrbute AttrObj = new Attrbute();
        Object ResData;
        string ClientId = "", ClientSecret = "", SubscriptionKey = "", data = "";
        string Resource = "", GrantType = "";

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/OAuth/getToken")]
        public HttpResponseMessage Token()
        {
            try
            {
                var req = Request.Headers;
                ClientId = req.GetValues("client_id").First();
                ClientSecret = req.GetValues("client_secret").First();
                SubscriptionKey = req.GetValues("Ocp-Apim-Subscription-Key").First();

                Resource = ConfigurationManager.AppSettings["ida:Audience"];
                GrantType = ConfigurationManager.AppSettings["ida:GrantType"];
            
                ResData = getToken(ClientId, ClientSecret, GrantType, Resource, SubscriptionKey);

                return Request.CreateResponse(HttpStatusCode.OK, ResData);
            }
            catch (Exception ex)
            {
                AttrObj.ErrMsg = ex.Message;
                ResData = JsonConvert.SerializeObject(AttrObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return Request.CreateResponse(HttpStatusCode.OK, ResData);
            }
        }

       


        public OutputResponse getToken(string strClientId, string strClientSecret, string strGrantType, string strResource, string strSubcription)
        {
            OutputResponse objOutRes = null;
            try
            {
                string URL = "https://login.microsoftonline.com/" + ConfigurationManager.AppSettings["ida:AuthTokenTenant"] + "/oauth2/token";
                var client = new RestClient(URL);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Ocp-Apim-Subscription-Key", strSubcription);
                request.AddParameter("grant_type", strGrantType);
                request.AddParameter("client_id", strClientId);
                request.AddParameter("client_secret", strClientSecret);
                request.AddParameter("resource", strResource);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    objOutRes = JsonConvert.DeserializeObject<OutputResponse>(response.Content);
                }

                else
                {
                    objOutRes = JsonConvert.DeserializeObject<OutputResponse>(response.Content);
                }
            }
            catch(Exception e)
            {
                objOutRes = new OutputResponse();
                objOutRes.ErrMsg = e.Message;
                return objOutRes;
            }
            return objOutRes;
        }
    }
}