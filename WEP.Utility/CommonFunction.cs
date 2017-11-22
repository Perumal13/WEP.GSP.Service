using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.CacheManager;
using WEP.GSP.Document;

namespace WEP.Utility
{
    public class CommonFunction
    {
       
        private static List<int> httpError = Constants.RetryHttpError.Split('|').Select(Int32.Parse).ToList();
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpErrorCode"></param>
        /// <returns></returns>
        public static bool ShouldRetry(int httpErrorCode)
        {          
           
            return httpError.Contains(httpErrorCode);                
        }

    }
}
