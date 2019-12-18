using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using CarStoreShared;

namespace CarStoreWeb.Data
{
    public class InventoryProxy : Proxy
    {
        private static Proxy _instance;

        public InventoryProxy(string endpoint, string key)
            :base(endpoint, key)
        {
            _instance = this;
        }

        public static InventoryProxy GetInstance()
        {
            if(_instance == null)
            {
                throw new InvalidOperationException("Singleton not instantiated at app startup");
            }

            return (InventoryProxy)_instance;
        }

        public Dictionary<string, CarItem> GetInventory()
        {
            HttpClient client = new HttpClient();

            var content = JsonSerializer.Serialize(new ApiPackage { ApiKey = _key });
            var externalTask = client.PostAsync($"{_endpoint}api/inventory", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return JsonSerializer.Deserialize<Dictionary<string, CarItem>>(returnedValue);
        }

        internal CarItem GetInventoryItem(string key)
        {
            HttpClient client = new HttpClient();

            var content = JsonSerializer.Serialize(new ApiPackage { ApiKey = _key });
            var externalTask = client.PostAsync($"{_endpoint}api/inventory/{key}", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return JsonSerializer.Deserialize<CarItem>(returnedValue);
        }
    }
}
