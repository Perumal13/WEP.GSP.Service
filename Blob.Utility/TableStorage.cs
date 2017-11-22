using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using WEP.GSP.Document;

namespace Blob.Utility
{
    public partial class TableStorage : Form
    {
        public TableStorage()
        {
            InitializeComponent();
            connection11.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage;AccountKey=k2lmSIVvPRLV70MjJXi12HM2yLdLXfhzvM+1PXWCaDG+f9dUZJvZhYrjjjoUAOtUsqZGT/4rpRApJmrdKNz3Qw==;EndpointSuffix=core.windows.net";
            connection12.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage1;AccountKey=uIjQF390ueXBt7nOz/kjTktRTt1gS9k1zHOVAnRmfv0I/zsfeknGywMVFiUbqca2ri98g7/tEQboYhG89fMGwA==;EndpointSuffix=core.windows.net";
            connection13.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage2;AccountKey=A/sWAbXH5nxxrtSAShfJpOW02vN+g0aBu9+hHL0w9a2paTFq43dtKZICZgkTqBJwFO4b9konTnUI3DkTwUUNlA==;EndpointSuffix=core.windows.net";
            connection14.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage3;AccountKey=MqN7unpF6iufw707kZsMhiRUQbdkVtrSwTzY7pFmpW5gTWdMOW1psiKQMsrYhrjMHPDrHKlXsbnUW78FjzXUjw==;EndpointSuffix=core.windows.net";
            connection15.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage4;AccountKey=btl9OuB0tbuHhicl2IgJWbq79NafzqBiKxJR47slTuCMbA3dI8H+3rKzcAlFflMq9hycIdD6hCtpI3xhKbOxXg==;EndpointSuffix=core.windows.net";
            connection16.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage5;AccountKey=KyVsc5/Eaby9hnMi2qhs4kwklSRajppXWJSUDRBk3GJm0XNhmxx+d3jcOHqCq6w981hZsdCZVVAFLVa67j7JxQ==;EndpointSuffix=core.windows.net";
            connection17.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage6;AccountKey=EcNXTGd4WqvQ+vRw77rdeLnm6ERoCujsRH804E4aQ0vQgnqZx/0uPIpFCmaVxitiavOVs2UeoOuysF2E9PNTBg==;EndpointSuffix=core.windows.net";
            connection18.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage7;AccountKey=JwUIg2OjA+fRJoZ+PL/Ozx5UtRhF1N8RQNBl6ZPBgQ0m7mygGq3MUahiPEy3vaIABGHqKBX2Ko/B7aRrEpubpA==;EndpointSuffix=core.windows.net";
            connection19.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage8;AccountKey=B1+SIEnaAtmSlboRUz4yS3hYbHsAgUoPuwW1s1jDu6pI5+J78sXnrgEc3kKDaCbn1DFIy+RvvTQIeR9kL88fyQ==;EndpointSuffix=core.windows.net";
            connection20.Text = "DefaultEndpointsProtocol=https;AccountName=wepstorage9;AccountKey=wYx7vxDlUaJqU5me+prFvb3G4UskwmULkFrrONXNtMbm39mFN3/VHT3jWn3CNfJg95v4QZMMlZ5JvgvFSMPaMg==;EndpointSuffix=core.windows.net";
            
            partition11.Text = "2017-15-07";
            partition12.Text = "2017-15-07";
            partition13.Text = "2017-15-07";
            partition14.Text = "2017-15-07";
            partition15.Text = "2017-15-07";
            partition16.Text = "2017-15-07";
            partition17.Text = "2017-15-07";
            partition18.Text = "2017-15-07";
            partition19.Text = "2017-15-07";
            partition20.Text = "2017-15-07";

        }

        private void connection11_TextChanged(object sender, EventArgs e)
        {

        }

        private int GetCountFromTable(string con, string partition1)
        {
            //string _StorageName = "GSTNStage";
            string _StorageName = "GSTR2AStage";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(con);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(_StorageName);

            table.CreateIfNotExists();

            //TableQuery<TableStorageEntity> query = new TableQuery<TableStorageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition1));

            TableQuery<TableStorageEntity> query = new TableQuery<TableStorageEntity>();

            int ClientIdCount = table.ExecuteQuery(query).Count();
            int LastEntry = table.ExecuteQuery(query).Count() - 1;

            return ClientIdCount;
        }

        private void TableStorage_Load(object sender, EventArgs e)
        {

        }

        private void txtTotal_Click(object sender, EventArgs e)
        {
            int c11 = GetCountFromTable(connection11.Text, partition11.Text);
            int c12 = GetCountFromTable(connection12.Text, partition12.Text);
            int c13 = GetCountFromTable(connection13.Text, partition13.Text);
            int c14 = GetCountFromTable(connection14.Text, partition14.Text);
            int c15 = GetCountFromTable(connection15.Text, partition15.Text);
            int c16 = GetCountFromTable(connection16.Text, partition16.Text);
            int c17 = GetCountFromTable(connection17.Text, partition17.Text);
            int c18 = GetCountFromTable(connection18.Text, partition18.Text);
            int c19 = GetCountFromTable(connection19.Text, partition19.Text);
            int c20 = GetCountFromTable(connection20.Text, partition20.Text);

            label1.Text = c11.ToString();
            label2.Text = c12.ToString();
            label3.Text = c13.ToString();
            label4.Text = c14.ToString();
            label5.Text = c15.ToString();
            label6.Text = c16.ToString();
            label7.Text = c17.ToString();
            label8.Text = c18.ToString();
            label9.Text = c19.ToString();
            label10.Text = c20.ToString();


            label11.Text = (c11 + c12 + c13 + c14 + c15 + c16 + c17 + c18 + c19 + c20).ToString();
        }

        private void bnDelete_Click(object sender, EventArgs e)
        {
            Delete(connection11.Text);
            Delete(connection12.Text);
            Delete(connection13.Text);
            Delete(connection14.Text);
            Delete(connection15.Text);
            Delete(connection16.Text);
            Delete(connection17.Text);
            Delete(connection18.Text);
            Delete(connection19.Text);
            Delete(connection20.Text);
            
        }

        private void Delete(string connection)
        {
            // Retrieve the storage account from the connection string.
            //string _StorageName = "GSTNStage";
            string _StorageName = "GSTR2AStage";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(_StorageName);

            // Delete the table it if exists.
            table.DeleteIfExists();
        }
    }
}
