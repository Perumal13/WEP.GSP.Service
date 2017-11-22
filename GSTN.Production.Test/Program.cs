using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace GSTN.Production.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("AuthToken : ");
                string authToken = Console.ReadLine();
                
                string resp = GSTR2Save(authToken);

                Console.WriteLine("Total Count1 :- {0}", resp);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :- {0} - {1}", ex.Message, ex.InnerException);
                Console.ReadLine();
                //throw ex;
            }
        }

        private static string GSTR2Save(string authToken)
        {

            string response;
            string jsonAttribute = "{\r\n  \"action\": \"RETSAVE\",\r\n  \"data\": \"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\",\r\n  \"hmac\": \"8A4F2iH07RNAwxfnxeqyjbhWefJXZDDZnygOqCvBLS0=\"\r\n}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Constants.ApiGSTR2);

            httpWebRequest.Timeout = 160000;

            httpWebRequest.ContentType = Constants.ContentType;
            httpWebRequest.Method = Constants.PUT;
            httpWebRequest.Accept = Constants.Accept;

            httpWebRequest.Headers.Add(Constants.Clientid, "l7xxdc73ba9d5ac34a159d1a398e943c594c");
            httpWebRequest.Headers.Add(Constants.State_cd, "29");
            httpWebRequest.Headers.Add(Constants.UserName, "ermsolutionska");
            httpWebRequest.Headers.Add(Constants.Txn, "returns");
            httpWebRequest.Headers.Add(Constants.Auth_Token, authToken);
            httpWebRequest.Headers.Add(Constants.Client_secret, "8f1bc912265849e5b5ffb5d014272cae");
            httpWebRequest.Headers.Add(Constants.IpUsr, "192.9.168.99");
            httpWebRequest.Headers.Add(Constants.Ret_period, "072017");
            httpWebRequest.Headers.Add(Constants.Gstin, "29AADCE9940P1ZT");

            var responseStream = httpWebRequest.GetRequestStream();

            using (var streamWriter = new StreamWriter(responseStream))
            {
                streamWriter.Write(jsonAttribute);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                int statuscode = (int)httpResponse.StatusCode;

                if (statuscode == 200)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd().ToString();

                        var results = JsonConvert.DeserializeObject<GstnResponse>(response);
                        results.username = "ermsolutionska";

                        response = JsonConvert.SerializeObject(results, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        return response;
                    }
                }
                
            }

            //var respone = new ServiceResponse<string> { ResponseObject = objResponse, IsError = false };
            return null;
        }
    }


    
}
