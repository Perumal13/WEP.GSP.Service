using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.Service.Blob
{
    public class TableStorage : IStorage
    {
        private string _jsonResp;
        private string _RequestToken;

        private string _OriginalError;
        private string _Exception;
        private string _StackTrace;

        private string _Response;
        private int _Stage;
        private string _PartitionKey;
        private string _RowKKey;
        private string _Clientid;
        private string _UserName;
        private string _ClientSecret;
        private string _Statecd;
        private string _Txn;
        private string _AuthToken;
        private string _IpUsr;

        private string _StorageName;
        private string _Time;

        public TableStorage(string partitionKey, string rowKey, int stage,string requestToken
                                                               , string storageName
                                                               , string currentTime)
        {
            this._PartitionKey = partitionKey;
            this._RowKKey = rowKey;
            this._Stage = stage;
            this._StorageName = storageName;
            this._Time = currentTime;
            this._RequestToken = requestToken;
        }

        public TableStorage(string partitionKey, string rowKey,  string requestToken
                                                               , string storageName
                                                               , string currentTime)
        {
            this._PartitionKey = partitionKey;
            this._RowKKey = rowKey;
            this._StorageName = storageName;
            this._Time = currentTime;
            this._RequestToken = requestToken;
        }

        //public TableStorage(string partitionKey, string rowKey, string reqjson, string requestToken
        //                                                        , string response
        //                                                        , string storageName)
        //{
        //    this._jsonResp = reqjson;
        //    this._RequestToken = requestToken;
        //    this._Response = response;
        //    this._PartitionKey = partitionKey;
        //    this._RowKKey = rowKey;
        //    this._StorageName = storageName;
        //}

        public TableStorage(string partitionKey, string rowKey, string requestToken
                                                              , int Stage
                                                              , string OriginalError
                                                              , string Exception
                                                              , string StackTrace
                                                              , string storageName
                                                              , string time)
        {
            this._Stage = Stage;
            this._RequestToken = requestToken;
            this._OriginalError = OriginalError;
            this._Exception = Exception;
            this._StackTrace = StackTrace;
            this._Time = time;

            this._StorageName = storageName;
            this._PartitionKey = partitionKey;
            this._RowKKey = rowKey;

        }

        public void InsertStageToTableStorage(string keys, int stage)
        {
            try
            {
                //string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(this._StorageName);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                string partitionKey = DateTime.Now.ToString("yyyy-mmm-dd");
                ////Since rowKey should be constant
                //string rowKey = DateTime.Now.Ticks.ToString();

                var rowKey = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 18) + DateTime.Now.Ticks.ToString();

                //var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                //var rowKey = guid.Substring(0, 18);

                // Create a new entity.
                ExTableStorageEntity tableStorageEntity = new ExTableStorageEntity(partitionKey, rowKey);

                tableStorageEntity.RequestToken = this._RequestToken;
                tableStorageEntity.Stage = stage;
                tableStorageEntity.Time = this._Time;

                // Create the TableOperation object that inserts the entity.
                TableOperation insertOperation = TableOperation.Insert(tableStorageEntity);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch (Exception es)
            {
                new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey
                                                        , Constants.RowKey
                                                        , this._RequestToken
                                                        , this._Stage
                                                        , es.Message
                                                        , es.Message
                                                        , es.StackTrace
                                                        , Constants.GSTNException
                                                        , Constants.currentTime
                                                       ).InsertToExTableStorage(es.Message);
            }
        }

        public TableStorage(string partitionKey, string rowKey, int stage
                                                        , string clientid
                                                        , string userName
                                                        , string clientSecret
                                                        , string statecd
                                                        , string txn
                                                        , string authToken
                                                        , string ipUsr
                                                        , string requestToken
                                                        , string storageName
                                                        , string time)
        {
            this._PartitionKey = partitionKey;
            this._RowKKey = rowKey;
            this._Stage = stage;
            this._Clientid = clientid;
            this._UserName = userName;
            this._ClientSecret = clientSecret;
            this._Statecd = statecd;
            this._AuthToken = authToken;
            this._IpUsr = ipUsr;
            this._RequestToken = requestToken;
            this._StorageName = storageName;
            this._Time = time;
        }

        /*
        public void Total_TableStorage_Count()
        {
            string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(value);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(_StorageName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            string partitionKey = this._PartitionKey;
            string rowKey = this._RowKKey;

            // Create a new entity.
            TableStorageEntity tableStorageEntity = new TableStorageEntity(partitionKey, rowKey);

            tableStorageEntity.Clientid = this._Clientid;

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TableStorageEntity> query = new TableQuery<TableStorageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this._PartitionKey));

            int ClientIdCount = table.ExecuteQuery(query).Count();

            Console.WriteLine("ClientId Count {0}", ClientIdCount);



        }

        */
        public bool ReadFromTableStorage(string clientid)
        {
            string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(value);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(_StorageName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            string partitionKey = this._PartitionKey;
            string rowKey = this._RowKKey;

            // Create a new entity.
            TableStorageEntity tableStorageEntity = new TableStorageEntity(partitionKey, rowKey);

            tableStorageEntity.Clientid = this._Clientid;




            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<TableStorageEntity> query = new TableQuery<TableStorageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this._PartitionKey));

            int ClientIdCount = table.ExecuteQuery(query).Count();
            int LastEntry = table.ExecuteQuery(query).Count() - 1;
            //int k = 1;
            //foreach (TableStorageEntity entity in table.ExecuteQuery(query))
            //{


            //    if (LastEntry == k)
            //    {
            //        Console.WriteLine("{0}, UserName={1}\t ClientId={2}", entity.PartitionKey, entity.UserName, entity.Clientid);
            //    }

            //    k++;
            //}

            Console.WriteLine("ClientId Count {0}", ClientIdCount);

            // Create a retrieve operation that takes a TableStorageEntity.
            TableOperation retrieveOperation = TableOperation.Retrieve<TableStorageEntity>(this._PartitionKey, this._RowKKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            TableStorageEntity TSE = RetrieveRecord(table, this._PartitionKey, this._RowKKey);

            if (TSE != null)
            {

                Console.WriteLine("Record exists");
            }
            else
            {
                Console.WriteLine("Record does not exists");
            }




            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Console.WriteLine("The entry exists.");
                return false;
            }
            else
            {
                Console.WriteLine("The entry number could not be retrieved.");
                return true;
            }

            // Execute the insert operation.
            // table.Execute(insertOperation);



        }


        public static TableStorageEntity RetrieveRecord(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation tableOperation = TableOperation.Retrieve<TableStorageEntity>(partitionKey, rowKey);
            TableResult tableResult = table.Execute(tableOperation);
            return tableResult.Result as TableStorageEntity;
        }

        public void InsertToTableStorage(string auditLogMessage)
        {
            string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(value);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(this._StorageName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            string partitionKey = DateTime.Now.ToString("MMMMyyyy");
            ////Since rowKey should be constant
            string rowKey = DateTime.Now.Ticks.ToString();

            //var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
            //var rowKey = guid.Substring(0, 18);

            // Create a new entity.
            TableStorageEntity tableStorageEntity = new TableStorageEntity(partitionKey, rowKey);
            tableStorageEntity.jsonResp = this._jsonResp;
            tableStorageEntity.RequestToken = this._RequestToken;
            tableStorageEntity.Response = this._Response;
            tableStorageEntity.Stage = this._Stage;
            tableStorageEntity.Clientid = this._Clientid;
            tableStorageEntity.UserName = this._UserName;
            tableStorageEntity.ClientSecret = this._ClientSecret;
            tableStorageEntity.Statecd = this._Statecd;
            tableStorageEntity.AuthToken = this._AuthToken;
            tableStorageEntity.IpUsr = this._IpUsr;
            tableStorageEntity.AuditMessage = auditLogMessage;
            tableStorageEntity.Time = this._Time;
            this._RowKKey = rowKey;
            tableStorageEntity.RowKey = this._RowKKey;
            tableStorageEntity.PartitionKey = this._PartitionKey;

            // Create the TableOperation object that inserts the entity.
            TableOperation insertOperation = TableOperation.Insert(tableStorageEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public void InsertToExTableStorage(string auditLogMessage)
        {
            try
            {
                string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(value);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(this._StorageName);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                string partitionKey = DateTime.Now.ToString("MMMMyyyy");
                ////Since rowKey should be constant
                string rowKey = DateTime.Now.Ticks.ToString();

                //var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                //var rowKey = guid.Substring(0, 18);

                //var rowKey = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 18);

                // Create a new entity.
                ExTableStorageEntity tableStorageEntity = new ExTableStorageEntity(partitionKey, rowKey);

                tableStorageEntity.RequestToken = this._RequestToken;
                tableStorageEntity.Stage = this._Stage;


                tableStorageEntity.RequestToken = this._RequestToken;
                tableStorageEntity.OriginalError = this._OriginalError;
                tableStorageEntity.Exception = this._Exception;
                tableStorageEntity.StackTrace = this._StackTrace;

                tableStorageEntity.Time = this._Time;
                tableStorageEntity.RowKey = this._RowKKey;
                tableStorageEntity.PartitionKey = this._PartitionKey;

                // Create the TableOperation object that inserts the entity.
                TableOperation insertOperation = TableOperation.Insert(tableStorageEntity);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch (Exception es)
            {
                new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey
                                                        , Constants.RowKey
                                                        , this._RequestToken
                                                        , this._Stage
                                                        , es.Message
                                                        , es.Message
                                                        , es.StackTrace
                                                        , Constants.GSTNException
                                                        , Constants.currentTime
                                                       ).InsertToExTableStorage(es.Message);
            }
        }

        public void InsertStageToTableStorage(string value)
        {
            try
            {
                //string value = Constants.TableStorageConnection;// ConfigurationManager.AppSettings["TableStorageConnectionString"].ToString();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(value);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                CloudTable table = tableClient.GetTableReference(this._StorageName);

                // Create the table if it doesn't exist.
                table.CreateIfNotExists();

                string partitionKey = DateTime.Now.ToString("yyyy-mmm-dd");
                ////Since rowKey should be constant
                //string rowKey = DateTime.Now.Ticks.ToString();

                var rowKey = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 18)+DateTime.Now.Ticks.ToString(); 

                //var guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                //var rowKey = guid.Substring(0, 18);

                // Create a new entity.
                ExTableStorageEntity tableStorageEntity = new ExTableStorageEntity(partitionKey, rowKey);

                tableStorageEntity.RequestToken = this._RequestToken;
                tableStorageEntity.Stage = this._Stage;
                tableStorageEntity.Time = this._Time;

                // Create the TableOperation object that inserts the entity.
                TableOperation insertOperation = TableOperation.Insert(tableStorageEntity);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }
            catch (Exception es)
            {
                new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey
                                                        , Constants.RowKey
                                                        , this._RequestToken
                                                        , this._Stage
                                                        , es.Message
                                                        , es.Message
                                                        , es.StackTrace
                                                        , Constants.GSTNException
                                                        , Constants.currentTime
                                                       ).InsertToExTableStorage(es.Message);
            }
        }

       
    }
}
