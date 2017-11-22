using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.Business
{
    public class NonStaticFunction
    {

        public int GetBlob()
        {
            int blobId = 10;
            blobId = blobId == 0 ? blobId + 10 : blobId;
            //return new CacheManager.CacheManager().GetBlob(blobId);
            return blobId;
        }

    }
}
