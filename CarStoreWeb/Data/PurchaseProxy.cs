using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using CarStoreShared;

namespace CarStoreWeb.Data
{
    public class PurchaseProxy : Proxy
    {
        private static Proxy _instance;

        public PurchaseProxy(string endpoint, string key)
            :base(endpoint, key)
        {
            _instance = this;
        }

        public static PurchaseProxy GetInstance()
        {
            if(_instance == null)
            {
                throw new InvalidOperationException("Singleton not instanitated at app startup");
            }

            return (PurchaseProxy)_instance;
        }

        public string TryOrder(string sessionIdentitfier, string email)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new ApiPackage { SessionIdentifier = sessionIdentitfier, ApiKey = _key, ContentItem = email });
            var externalTask = client.PostAsync($"{_endpoint}api/purchase", new StringContent(content, Encoding.UTF8, "application/json"));
            var returnedValue = "Failed;A technical problem occured";

            try
            {
                externalTask.Wait();
                var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
                returnedValueTask.Wait();
                returnedValue = returnedValueTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }

            return returnedValue;
        }
    }
}
