using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.Service.Blob
{
    public class BlobWithRetry
    {

        private object _payloadAttribute;
        private readonly string _connection;
        private readonly string _container;
        private readonly string _blobFile;
        private readonly Dictionary<string, string> _metadata;
        private readonly int _blobAccId; // renme to blobAccountId
        private static Dictionary<int, CloudBlobContainer> cloudBlobClientDic = new Dictionary<int, CloudBlobContainer>();

        public BlobWithRetry(string connection, string container, string blobFile
                         , Dictionary<string, string> metadata
                           , int blobAccId)
        {
            this._connection = connection;
            this._container = container;
            this._blobFile = blobFile;
            this._metadata = metadata;
            this._blobAccId = blobAccId;
        }

        public Attrbute Download()
        {
            var container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(this._blobFile);

            using (var stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                stream.Position = 0;//resetting stream's position to 0
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<Attrbute>(jsonTextReader);
                    }
                }
            }
        }

        public GstnResponse DownloadGstnBlob()
        {
            var container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(this._blobFile);

            using (var stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                stream.Position = 0;//resetting stream's position to 0
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<GstnResponse>(jsonTextReader);
                    }
                }
            }
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobContainer container = null;
            //Check if object exist for given blobid in Dictionary
            if (cloudBlobClientDic.TryGetValue(this._blobAccId, out container))
            {
                return container;
            }
            else
            {    //Check  object doesn't exist for given blobid
                var storage = CloudStorageAccount.Parse(this._connection);
                CloudBlobClient blobClient = storage.CreateCloudBlobClient();

                // Handle “Transient” Linear Retry Policy
                IRetryPolicy linearRetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(Constants.DelayTrialms), Constants.MaxTrial);
                blobClient.DefaultRequestOptions.RetryPolicy = linearRetryPolicy;

                container = blobClient.GetContainerReference(this._container);

                if (!container.Exists())
                    container.Create(); // Review : exception handling while Creating container

                //Dictionary add the object to dictionary
                cloudBlobClientDic.Add(this._blobAccId, container);
                return container;
            }
        }

        public Attrbute DownloadBlobRetry()
        {
            Attrbute objAttrbute = new Attrbute();
            int remainingTries = Constants.MaxBlobDownloadTrial;
            var exceptions = new List<Exception>();

            while (remainingTries > 0)
            {
                try
                {
                    return objAttrbute= Download();
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
                //if (remainingTries == 0) //Circit breaker
                //{
                //    new RequestData().InsertBacklogRequest(this._reqtoken, this._username, jsonAttribute);
                //}
            }

            throw new AggregateException("Could not process request for donload. Will be re-attempt after some time", exceptions);
        }

        public GstnResponse DownloadGStnRespBlobRetry()
        {
            GstnResponse objGstnResponse = new GstnResponse();
            int remainingTries = Constants.MaxBlobDownloadTrial;
            var exceptions = new List<Exception>();

            while (remainingTries > 0)
            {
                try
                {
                    return objGstnResponse = DownloadGstnBlob();
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
                //if (remainingTries == 0) //Circit breaker
                //{
                //    new RequestData().InsertBacklogRequest(this._reqtoken, this._username, jsonAttribute);
                //}
            }

            throw new AggregateException("Could not process request for donload. Will be re-attempt after some time", exceptions);
        }
    }
}
