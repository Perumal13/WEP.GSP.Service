using System.Web;
using System.Web.Mvc;

namespace WEP.GSP.ServiceBus.RestAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
