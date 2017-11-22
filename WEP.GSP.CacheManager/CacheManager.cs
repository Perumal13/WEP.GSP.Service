using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WEP.GSP.Data;
using WEP.GSP.Document;

namespace WEP.GSP.CacheManager
{
    public class CacheManager
    {
        static Dictionary<int, Blob> blobDict;
        static Dictionary<int, Blob> blobDictGstnResp;
        public void Delete(string cacheId)
        {
            try
            {
                HttpRuntime.Cache.Remove(cacheId);
            }
            catch
            {
                //do nothing
            }
        }


        //public static void ExpireBookmarkJob(long userId)
        //{
        //    var key = "glm_BookmarkJob_" + userId;
        //    Delete(key);
        //    GetBookmarkJob(userId);
        //}


        //Get BlobConfig Details
        public Blob GetBlob(int Id)
        {
            //var key = "Gsp_Blob";
            if (blobDict == null)
            {
                blobDict = new ConfigData().GetGstnReqBlob();
                //CacheHelper.Add(blobDict, key);
            }

            Blob blob = null;
            if (blobDict != null)
            blobDict.TryGetValue(Id, out blob);

            return blob;
        }

        public Blob GetGStnRespBlob(int Id)
        {
            //var key = "Gsp_Blob";
            if (blobDictGstnResp == null)
            {
                blobDictGstnResp = new ConfigData().GetGStnRespBlob();
                //CacheHelper.Add(blobDict, key);
            }

            Blob blob = null;
            if (blobDictGstnResp != null)
                blobDictGstnResp.TryGetValue(Id, out blob);

            return blob;
        }

        public int GetBlobAccountCount()
        {
            //var key = "Gsp_Blob";
            if (blobDict == null)
            {
                blobDict = new ConfigData().GetGstnReqBlob();
                //CacheHelper.Add(blobDict, key);
            }            
            return blobDict.Count() ;
        }
    }
}

