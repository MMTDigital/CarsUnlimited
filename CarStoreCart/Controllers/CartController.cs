using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreCart.Data;
using CarStoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarStoreCart.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ApiOptions _apiOptions;
        private CartBroker _broker;

        public CartController(IOptions<ApiOptions> options)
        {
            _broker = new CartBroker();
            _apiOptions = options.Value;
        }

        [HttpPost]
        public string Post([FromBody]ApiPackage package)
        {
            var sessionId = package.SessionIdentifier;
            var apiKey = package.ApiKey;

            if(sessionId == null || apiKey == null)
            {
                throw new InvalidOperationException("API package invalid");
            }

            if(!apiKey.Equals(_apiOptions.MyApiKey))
            {
                throw new InvalidOperationException("API Key not valid");
            }

            return _broker.GetItemsInCart(_apiOptions.StorageConnectionString, sessionId);
        }

        [HttpPut("{id}")]
        public string Put(string id, [FromBody]ApiPackage package)
        {
            var sessionId = package.SessionIdentifier;
            var apiKey = package.ApiKey;
            var sessionIdGet = id;

            if(sessionId == null || apiKey == null)
            {
                throw new InvalidOperationException("API package invalid");
            }

            if(!apiKey.Equals(_apiOptions.MyApiKey))
            {
                throw new InvalidOperationException("API Key invalid");
            }

            if(!sessionIdGet.Equals(sessionId))
            {
                throw new InvalidOperationException("Session IDs do not match");
            }

            int newNumberOfItems = _broker.ChangeItemInCart(_apiOptions.StorageConnectionString, sessionId, package.ContentItem);

            return newNumberOfItems.ToString();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _broker.EmptyCart(_apiOptions.StorageConnectionString, id);
        }
    }
}
