using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace WEP.GSP.CacheManager
{
  
        internal static class CacheHelper
        {

            public static void Add<T>(T o, string key)
            {
                HttpContext.Current.Cache.Insert(
                    key,
                    o,
                    null,
                    DateTime.Now.AddMinutes(1440),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }

            public static void Clear(string key)
            {
                HttpContext.Current.Cache.Remove(key);
            }

            public static bool Exists(string key)
            {
            if (HttpContext.Current == null)
                return false;

                return HttpContext.Current.Cache[key] != null;
            }

            public static bool Get<T>(string key, out T value)
            {
                try
                {
                    if (!Exists(key))
                    {
                        value = default(T);
                        return false;
                    }

                    value = (T)HttpContext.Current.Cache[key];
                }
                catch
                {
                    value = default(T);
                    return false;
                }

                return true;
            }
        }
    }
