using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.CacheManager;
using WEP.GSP.Document;

namespace WEP.GSP.Business
{
    public class CommonFunction
    {
        //static int blobId;
        //private static int blobAccountCount = new CacheManager.CacheManager().GetBlobAccountCount ();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspUserId"></param>
        /// <returns></returns>
        public static string GetUniqueToken(string clientid, string username, string statecd, string ipUsr)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}"
                                , Guid.NewGuid()  // try using pre created Guid
                                , clientid
                                , username
                                , statecd
                                , ipUsr
                                );
        }

        public static string GetUniqueToken()
        {
            //, string ipUsr
            return String.Format("{0}_{1}"
                                , Guid.NewGuid()  // try using pre created Guid
                                , DateTime.Now.Ticks
                                );
        }

        /// <summary>
        /// Get BlobConfig Details
        /// </summary>
        /// <returns></returns>
        public static Blob GetBlob()
        {
            int ReqStorageAcc = Constants.ReqStorageAccont;
            int blobId = (new Random().Next(1, 9999))% ReqStorageAcc;
            blobId = blobId == 0 ? blobId + Constants.ReqStorageAccont : blobId;
            return new CacheManager.CacheManager().GetBlob(blobId);
        }

        public static Blob GetBlobById(int blobId)
        {
            return new CacheManager.CacheManager().GetBlob(blobId);
        }

        public static Blob GetGstnRespBlob()
        {
            int RespStorageAcc = Constants.RespStorageAccont;
            int blobId = (new Random().Next(1, 9999)) % RespStorageAcc;
            //blobId = blobId == 0 ? blobId + 5 : blobId;
            blobId = blobId +100;
            return new CacheManager.CacheManager().GetGStnRespBlob(blobId);
        }
    }
}
