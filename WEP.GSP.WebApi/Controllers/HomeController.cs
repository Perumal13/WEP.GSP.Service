
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.IO;
using System.Web.Script.Serialization;
using WEP.GSP.Document;
using WEP.GSP.Business;
using System.Net.Http;
using Newtonsoft.Json;

namespace WEP.GSP.WebApi.Controllers
{
    public class HomeController : Controller
    {

         private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;


       // private readonly string _authToken;
        private  string _authToken;
        private string _clientid;
        private string _statecd;
        private string _username;
        private string _txn;
        private string _clientSecret;
        private string _ipUsr;


        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "http://localhost:2277/Home/About" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }


            return View();
        }


        public void SignOut()
        {
            // Send an OpenID Connect sign-out request.
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Name = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            ViewBag.ObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            ViewBag.GivenName = ClaimsPrincipal.Current.FindFirst(ClaimTypes.GivenName).Value;
            ViewBag.Surname = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Surname).Value;
            // ViewBag.UPN = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Upn).Value;

            return View();
        }


        [Authorize]
        public ActionResult Contact()
        {


            string url = Url.RouteUrl(
            "DefaultApi",
            new { httproute = "", controller = "Account" } );

            ViewBag.Name = "Girish Babu C s/o Chitti Babu";
            ViewBag.Address = "Mandya";

            return View();
        }


        [Authorize]
        public ServiceResponse<string> SaveGSTRI(WEP.GSP.Document.Attrbute objAttr)
        {

            var token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken)
                                             .SaveGSTR1(objAttr);

            var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };

            //Serialize to string
            string jsonReqst = new JavaScriptSerializer().Serialize(respone);

            return respone;

        }



        [Authorize]
        public JsonResult GetAll(WEP.GSP.Document.Attrbute objAttr)
        {
            string jsonRequest = "{\"PartitionKey\":\"PK_SaveGSTR1\",\"RequestType\":1,\"Clientid\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60\",\"Statecd\":\"11\",\"Username\":\"WeP\",\"Txn\":\"returns\",\"ClientSecret\":\"30a28162eb024f6e859a12bbb9c31725\",\"IpUsr\":\"12.8.9l.80\",\"Blob\":2,\"BlobFile\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636292360674179616\",\"RequestToken\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636292360674179616\",\"AuthToken\":\"8a227e0ba56042a0acdf98b3477d2c03\",\"CreatedOn\":\"5/1/2017 11:47:52 AM\",\"Response\":null,\"ModifiedOn\":null}";
            var request = JsonConvert.DeserializeObject<Request>(jsonRequest);

            var token = new GSTR1Business(this._clientid,
                                               this._statecd,
                                               this._username,
                                               this._txn,
                                               this._clientSecret,
                                               this._ipUsr,
                                               this._authToken)
                                             .SaveGSTR1(objAttr);

            var respone = new ServiceResponse<string> { ResponseObject = token, IsError = false };

            
            var result = new JsonResult
            {
                Data = JsonConvert.DeserializeObject(token)
            };


            return result;
        }


        [Authorize]
        public ActionResult SampleForm()
        {

            return View();
        }
    }
}
