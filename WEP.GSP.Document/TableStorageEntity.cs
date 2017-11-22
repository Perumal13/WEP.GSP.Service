using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEP.GSP.Document
{
    public class ExTableStorageEntity : TableEntity
    {
        public ExTableStorageEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public ExTableStorageEntity() { }

        public string RequestToken { get; set; }
        //public string Response { get; set; }
        public int Stage { get; set; }
        public string OriginalError { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string Time { get; set; }
    }


    public class TableStorageEntity : TableEntity
    {
        public TableStorageEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public TableStorageEntity() { }

        public string jsonResp { get; set; }
        public string RequestToken { get; set; }
        public string Response { get; set; }
        public int Stage { get; set; }
        public string Clientid { get; set; }
        public string UserName { get; set; }
        public string ClientSecret { get; set; }
        public string Statecd { get; set; }
        public string AuthToken { get; set; }
        public string IpUsr { get; set; }
        public string AuditMessage { get; set; }
        public string Time { get; set; }
    }
}
