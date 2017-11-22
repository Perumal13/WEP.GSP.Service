using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WEP.GSP.Data;
using WEP.GSP.Document;
using WEP.GSP.Service.Blob;

namespace WEP.GSP.Business
{
    public class BlobRetry
    {
        public string UploadBlobWithRetry(Request request)
        {
            int blobId = request.Blob;
            //int remainingTries = Constants.MaxTrial;
            int remainingTries = 10;
            var exceptions = new List<Exception>();
            string jsonRequest = new JavaScriptSerializer().Serialize(request);

            while (remainingTries > 0)
            {
                try
                {
                    //Get blob storage account connection string randomly
                    var blob = new WEP.GSP.CacheManager.CacheManager().GetBlob(blobId);

                    //Upload gstn response to blobstorage 
                    var blobStorage = new BlobStorage(blob.Connection,
                                                      Constants.GstnResponseContainer,
                                                      request.RequestToken,
                                                      new Dictionary<string, string>(),
                                                      blob.Id);

                    var blobPath = blobStorage.UploadBlob(request);

                    blobId = (blobId+1) % 10;

                    //blobId += 1 % 10;

                    return blobPath;
                }
                catch (TimeoutException e)
                {
                    exceptions.Add(e);
                }
                catch (CustomException cex)
                {
                    exceptions.Add(cex);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                Thread.Sleep(Constants.DelayTrialms);

                remainingTries--;
                if (remainingTries == 0) //Circit breaker
                {
                    new RequestData().InsertBacklogRequest(request.RequestToken, request.Username, jsonRequest);
                }
            }

            throw new AggregateException("Could not process request. Will be re-attempt after some time", exceptions);
        }

    }
}
