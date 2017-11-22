using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WEP.GSP.Business;
using WEP.GSP.Document;
using WEP.GSP.Service.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int c = 0;

            //Attributes obj = new Attributes
            //{
            //    action = "RETSAVE",
            //    data = "sdfsdfsdfsdf",
            //    hmac = "sdfsdfsdf"

            //};
            //TestUploadToBlobStorage();
            //var json = new JavaScriptSerializer().Serialize(obj);
            //Console.WriteLine(json); 
            Console.ReadLine();  
        }

        //public static void TestUploadToBlobStorage()
        //{
        //    var obj = new
        //    {
        //        By = "Alexandre Brisebois",
        //        Message = "This is a test Message"
        //    };

        //    var uploadToBlobStorage = new BlobStorage(obj,
        //                                            "glamioasset",
        //                                            "testObject",
        //                                            new Dictionary<string, string>());

        //    var storageAccount = CloudStorageAccount.Parse(GetConnectionString());

        //  //  uploadToBlobStorage.Apply(storageAccount);
        //}

       
        //public void TestUploadThenChangeThenUpload()
        //{
        //    var obj = new
        //    {
        //        By = "Alexandre Brisebois",
        //        Message = "This is a test Message"
        //    };

        //    var uploadToBlobStorage = new BlobStorage(obj,
        //                                            "test",
        //                                            "testObject",
        //                                            new Dictionary<string, string>());

        //    var storageAccount = CloudStorageAccount.Parse(GetConnectionString());

        //  //  uploadToBlobStorage.Apply(storageAccount);

        //    obj = new
        //    {
        //        By = "Charles Brisebois",
        //        Message = "This is a test Message"
        //    };

        //    uploadToBlobStorage = new BlobStorage(obj,
        //                                            "test",
        //                                            "testObject",
        //                                            new Dictionary<string, string>());

        //   // uploadToBlobStorage.Apply(storageAccount);
        //}

        //private static string GetConnectionString()
        //{
        //    return ConfigurationManager.AppSettings["StorageConnectionString"].ToString();
        //}
    }
}
