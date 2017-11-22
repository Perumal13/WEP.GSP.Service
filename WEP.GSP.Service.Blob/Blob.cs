using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using WEP.GSP.Document;

namespace WEP.GSP.Service.Blob
{
    public class BlobStorage : IStorage
    {
        private object _payloadAttribute;
        private readonly string _connection;
        private readonly string _container;
        private readonly string _blobFile;
        private readonly Dictionary<string, string> _metadata;
        private readonly int _blobAccId; // renme to blobAccountId
        private static Dictionary<int, CloudBlobContainer> cloudBlobClientDic = new Dictionary<int, CloudBlobContainer>();   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="container"></param>
        /// <param name="blobAddressUri"></param>
        /// <param name="metadata"></param>
        public BlobStorage(string connection, string container, string blobFile
                          , Dictionary<string, string> metadata
                            , int blobAccId)
        {
            this._connection = connection;
            this._container = container;
            this._blobFile = blobFile;
            this._metadata = metadata;
            this._blobAccId = blobAccId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobContainer container = null;
            //Check if object exist for given blobid in Dictionary
            if (cloudBlobClientDic.TryGetValue(this._blobAccId, out container))
            {
                return container;
            } else
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobFilel"></param>
        /// <returns></returns>
        public Attrbute Download()
        {
            var container  = GetCloudBlobContainer();
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

          /// <summary>
        /// 
        /// </summary>
        /// <param name="blobReference"></param>
        private void SetBlobProperties(CloudBlockBlob blobReference)
        {
            blobReference.Properties.ContentType = Constants.ContentType;
            foreach (var meta in _metadata)
            {
                blobReference.Metadata.Add(meta.Key, meta.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        private void LoadStreamWithJson(Stream ms)
        {
            var json = JsonConvert.SerializeObject(this._payloadAttribute);
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(json);
            writer.Flush();
            ms.Position = 0;
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payloadAttribute"></param>
        /// <returns></returns>
        public string UploadBlob(object payloadAttribute)
        {
            //Tech - throttling error handling
            //Tech - 

            //async Task      

                this._payloadAttribute = payloadAttribute;
                if (this._blobFile == null || this._blobFile.Length == 0)
                {
                    throw new CustomException(
                                            String.Format("Error Message : {0}", "No Payload. Payload is empty"), String.Format("Error Source :- {0}", "WEP.GSP.Service.Blob.BlobStorage.UploadBlobAsync")
                                             , (int)WEP.GSP.Document.Stage.Blob_Upload_Error);
                }

                var container = GetCloudBlobContainer();

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(_blobFile);

                return UploadToContainer(cloudBlockBlob);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockBlob"></param>
        /// <returns></returns>
        private string UploadToContainer(CloudBlockBlob blockBlob)
        {
            //async Task<string>
            SetBlobProperties(blockBlob);
            using (var ms = new MemoryStream())
            {
                LoadStreamWithJson(ms);

                blockBlob.UploadFromStream(ms);

                return blockBlob.Uri.AbsoluteUri;
            }
        }  
    }
}
