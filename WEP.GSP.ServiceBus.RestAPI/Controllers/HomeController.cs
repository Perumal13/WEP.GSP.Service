using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEP.GSP.Business;
using WEP.GSP.Document;
namespace WEP.GSP.ServiceBus.RestAPI.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public GstnResponse DownloadGstn()
        {
            string token = "a3850a97-3011-425a-8227-ff7d222c8f88_636376236940662300";

            GstnResponse objGstnResponse = new GstnResponse();
            UserBusiness objBusiness = new UserBusiness();
            Attrbute objAttrbute = new Attrbute();
            Blob objBlob = new Blob();

            objGstnResponse = objBusiness.GetBlobConnectionByToken(token);

            



            return objGstnResponse;
        }

        

    }

    
}
