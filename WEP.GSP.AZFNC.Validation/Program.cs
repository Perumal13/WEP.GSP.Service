using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Business;


namespace WEP.GSP.AZFNC.Validation
{
    class Program
    {
        //static string myEventHubMessage = string.Empty;
        //static string myEventHubMessage = "{ \"RequestToken\": \"aasdfgdfga\", \"RequestType\":1, \"Clientid\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60\", \"Statecd\":\"11\", \"Username\":\"WeP\", \"Txn\":\"returns\", \"ClientSecret\":\"30a28162eb024f6e859a12bbb9c31725\", \"IpUsr\":\"12.8.9l.80\", \"Blob\":1, \"BlobFile\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636286450812343303\", \"AuthToken\":\"8a227e0ba56042a0acdf98b3477d2c03\" }";
        static string myEventHubMessage = "{\"PartitionKey\":\"PK_SaveGSTR1\",\"RequestType\":1,\"Clientid\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60\",\"Statecd\":\"11\",\"Username\":\"WeP\",\"Txn\":\"returns\",\"ClientSecret\":\"30a28162eb024f6e859a12bbb9c31725\",\"IpUsr\":\"12.8.9l.80\",\"Blob\":2,\"BlobFile\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636305444387777979\",\"RequestToken\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636305444387777979\",\"AuthToken\":\"8a227e0ba56042a0acdf98b3477d2c03\",\"CreatedOn\":\"5/1/2017 11:47:52 AM\",\"Response\":null,\"ModifiedOn\":null}";
        static void Main(string[] args)
        {
            try
            {
                new GSTR1Business().ValidateGSTR1(myEventHubMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("", ex.Message);
            }
            ////log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

            //////var data = JsonConvert.DeserializeObject<Request>(myEventHubMessage);
            //////log.Info($"After deserialization message: {data}");

            ////if (myEventHubMessage == string.Empty)
            ////{
            ////    log.Info($"EventHub is Empty : ");
            ////}
            ////else if (myEventHubMessage == "Test Message")
            ////{
            ////    log.Info($"Testing .... ");
            ////}
            ////else
            ////{
            //    string EhConnectionString_Write = "Endpoint=sb://test-eh-namespace.servicebus.windows.net/;SharedAccessKeyName=Write;SharedAccessKey=my+nz279gDekVcrAEAQ9j6i51vrBRAXil5u49JZ7AvM=;EntityPath=GSTN-REQ-Event-HUB";
            //    string EhEntityPath = "GSTN-REQ-Event-HUB";

            //    var connectionStringBuilder = new EventHubsConnectionStringBuilder(EhConnectionString_Write)
            //    {
            //        EntityPath = EhEntityPath
            //    };

            //    EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            //    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(myEventHubMessage)));
            //    await eventHubClient.CloseAsync();
            ////}

           

            new GSTR1Business().ValidateGSTR1(myEventHubMessage);
        }
    }
}
