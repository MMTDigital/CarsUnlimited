using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreInventory.Data;
using CarStoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarStoreInventory.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly ApiOptions _apiOptions;
        private InventoryBroker _broker;

        public InventoryController(IOptions<ApiOptions> apiOptions)
        {
            _apiOptions = apiOptions.Value;
            _broker = new InventoryBroker();
            _broker.PullAsync(_apiOptions.StorageConnectionString).Wait();
        }

        [HttpPost]
        public Dictionary<string, CarItem> Post([FromBody]ApiPackage package)
        {
            var sessionId = package.SessionIdentifier;
            var apiKey = package.ApiKey;

            if(apiKey == null)
            {
                return new Dictionary<string, CarItem>();
            }

            if(!apiKey.Equals(_apiOptions.MyApiKey))
            {
                return new Dictionary<string, CarItem>();
            }

            return _broker.GetCurrentInventory();
        }

        [HttpPost("{id}")]
        public CarItem PostItem(string id, [FromBody]ApiPackage package)
        {
            var sessionId = package.SessionIdentifier;
            var apiKey = package.ApiKey;

            if(apiKey == null)
            {
                return null;
            }

            if(!apiKey.Equals(_apiOptions.MyApiKey))
            {
                return null;
            }

            if(string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            if (!id.Contains(":"))
            {
                return null;
            }

            return _broker.GetInventoryItemById(id);
        }

        [HttpPut]
        public string Put([FromBody]ApiPackage package)
        {
            return _broker.ChangeStock(package.ContentItem, _apiOptions.StorageConnectionString);
        }
    }
}
