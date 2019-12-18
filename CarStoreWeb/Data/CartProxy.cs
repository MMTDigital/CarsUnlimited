using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using CarStoreShared;

namespace CarStoreWeb.Data
{
    public class CartProxy : Proxy
    {
        private static Proxy _instance;

        public CartProxy(string endpoint, string key)
            : base(endpoint, key)
        {
            _instance = this;
        }

        public static CartProxy GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Singelton not instantiated at app startup.");
            }

            return (CartProxy)_instance;
        }

        public int GetCurrentNumberOfItems(string sessionIdentifier)
        {
            HttpClient client = new HttpClient();

            var content = JsonSerializer.Serialize(new ApiPackage { SessionIdentifier = sessionIdentifier, ApiKey = _key });
            var externalTask = client.PostAsync($"{_endpoint}api/cart", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue.Split(";")[0]);
        }

        public string[] GetCurrentCartItems(string sessionIdentifier)
        {
            HttpClient client = new HttpClient();
            var content = JsonSerializer.Serialize(new ApiPackage { SessionIdentifier = sessionIdentifier, ApiKey = _key });
            var externalTask = client.PostAsync($"{_endpoint}api/cart", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return returnedValue.Split(";")[1].Split(",");
        }

        public int AddItem(string sessionIdentifier, string manufacturerAndIdString)
        {
            HttpClient client = new HttpClient();

            var content = JsonSerializer.Serialize(new ApiPackage { SessionIdentifier = sessionIdentifier, ApiKey = _key, ContentItem = manufacturerAndIdString });
            var externalTask = client.PostAsync($"{_endpoint}api/cart/{sessionIdentifier}", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue);
        }

        public int RemoveItem(string sessionIdentifier, string manufacturerAndIdString)
        {
            HttpClient client = new HttpClient();

            var content = JsonSerializer.Serialize(new ApiPackage { SessionIdentifier = sessionIdentifier, ApiKey = _key, ContentItem = $"-{manufacturerAndIdString}" });
            var externalTask = client.PutAsync($"{_endpoint}api/cart/{sessionIdentifier}", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return int.Parse(returnedValue);
        }
    }
}
