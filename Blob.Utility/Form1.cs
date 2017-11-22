using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEP.GSP.Document;

namespace Blob.Utility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            connection1.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage;AccountKey=k2lmSIVvPRLV70MjJXi12HM2yLdLXfhzvM+1PXWCaDG+f9dUZJvZhYrjjjoUAOtUsqZGT/4rpRApJmrdKNz3Qw==";
            connection2.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage1;AccountKey=uIjQF390ueXBt7nOz/kjTktRTt1gS9k1zHOVAnRmfv0I/zsfeknGywMVFiUbqca2ri98g7/tEQboYhG89fMGwA==";
            connection3.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage2;AccountKey=A/sWAbXH5nxxrtSAShfJpOW02vN+g0aBu9+hHL0w9a2paTFq43dtKZICZgkTqBJwFO4b9konTnUI3DkTwUUNlA==";
            connection4.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage3;AccountKey=MqN7unpF6iufw707kZsMhiRUQbdkVtrSwTzY7pFmpW5gTWdMOW1psiKQMsrYhrjMHPDrHKlXsbnUW78FjzXUjw==";
            connection5.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage4;AccountKey=btl9OuB0tbuHhicl2IgJWbq79NafzqBiKxJR47slTuCMbA3dI8H+3rKzcAlFflMq9hycIdD6hCtpI3xhKbOxXg==";
            connection6.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage5;AccountKey=KyVsc5/Eaby9hnMi2qhs4kwklSRajppXWJSUDRBk3GJm0XNhmxx+d3jcOHqCq6w981hZsdCZVVAFLVa67j7JxQ==";
            connection7.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage6;AccountKey=EcNXTGd4WqvQ+vRw77rdeLnm6ERoCujsRH804E4aQ0vQgnqZx/0uPIpFCmaVxitiavOVs2UeoOuysF2E9PNTBg==";
            connection8.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage7;AccountKey=JwUIg2OjA+fRJoZ+PL/Ozx5UtRhF1N8RQNBl6ZPBgQ0m7mygGq3MUahiPEy3vaIABGHqKBX2Ko/B7aRrEpubpA==";
            connection9.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage8;AccountKey=B1+SIEnaAtmSlboRUz4yS3hYbHsAgUoPuwW1s1jDu6pI5+J78sXnrgEc3kKDaCbn1DFIy+RvvTQIeR9kL88fyQ==";
            connection10.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage9;AccountKey=wYx7vxDlUaJqU5me+prFvb3G4UskwmULkFrrONXNtMbm39mFN3/VHT3jWn3CNfJg95v4QZMMlZ5JvgvFSMPaMg==";
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //int rnd = new Random().Next(1, 99999);
            //int blobId = (rnd) % 10;
            //MessageBox.Show(rnd.ToString() + " < > " + blobId.ToString());
            //return;
            int c1 = GetCount(connection1.Text);
            int c2 = GetCount(connection2.Text);
            int c3 = GetCount(connection3.Text);
            int c4 = GetCount(connection4.Text);
            int c5 = GetCount(connection5.Text);
            int c6 = GetCount(connection6.Text);
            int c7 = GetCount(connection7.Text);
            int c8 = GetCount(connection8.Text);
            int c9 = GetCount(connection9.Text);
            int c10 = GetCount(connection10.Text);

            count1.Text = c1.ToString();
            Count2.Text = c2.ToString();
            Count3.Text = c3.ToString();
            Count4.Text = c4.ToString();
            Count5.Text = c5.ToString();
            Count6.Text = c6.ToString();
            Count7.Text = c7.ToString();
            Count8.Text = c8.ToString();
            Count9.Text = c9.ToString();
            Count10.Text = c10.ToString();

            lbltotal.Text = (c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8 + c9 + c10).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private int GetCount(string connection)
        {
            if (connection.Trim() == "")
                return 0;
            try
            {
                CloudBlobContainer container = GetContainer(connection, "payload");
                var count = container.ListBlobs().Count();
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private bool Delete(string connection)
        {
            if (connection.Trim() == "")
                return false;
            try
            {
                CloudBlobContainer container = GetContainer(connection, "payload");
                container.Delete();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_connection"></param>
        /// <param name="_container"></param>
        /// <returns></returns>
        private CloudBlobContainer GetContainer(string _connection, string _container)
        {
            CloudBlobContainer container = null;

            _connection = _connection.Trim();
            var storage = CloudStorageAccount.Parse(_connection);
            CloudBlobClient blobClient = storage.CreateCloudBlobClient();

            // Handle “Transient” Linear Retry Policy
            IRetryPolicy linearRetryPolicy = new LinearRetry(TimeSpan.FromSeconds(2), 10);
            blobClient.DefaultRequestOptions.RetryPolicy = linearRetryPolicy;

            container = blobClient.GetContainerReference(_container);
            
            return container;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Delete(connection1.Text);
            Delete(connection2.Text);
            Delete(connection3.Text);
            Delete(connection4.Text);
            Delete(connection5.Text);
            Delete(connection6.Text);
            Delete(connection7.Text);
            Delete(connection8.Text);
            Delete(connection9.Text);
            Delete(connection10.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       

        private int GetCountFromTable(string connection11, string partition1)
        {
            string _StorageName = "GSTNStage";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection11);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(_StorageName);

            TableQuery<TableStorageEntity> query = new TableQuery<TableStorageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition1));

            int ClientIdCount = table.ExecuteQuery(query).Count();
            int LastEntry = table.ExecuteQuery(query).Count() - 1;

            return ClientIdCount;
        }
    }
}
