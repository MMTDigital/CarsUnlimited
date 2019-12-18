using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStorePurchase.Data;
using CarStoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarStorePurchase.Controllers
{
    [Route("api/[controller]")]
    public class PurchaseController : Controller
    {
        private readonly ApiOptions _apiOptions;
        private readonly PurchaseOptions _purchaseOptions;
        private PurchaseBroker _broker;


        public PurchaseController(IOptions<ApiOptions> apiOptions, IOptions<PurchaseOptions> purchaseOptions)
        {
            _apiOptions = apiOptions.Value;
            _purchaseOptions = purchaseOptions.Value;
            _broker = new PurchaseBroker();
        }

        [HttpPost]
        public string Post([FromBody]ApiPackage package)
        {
            var result = _broker.CheckoutCart(package.SessionIdentifier, package.ContentItem,
                _purchaseOptions.SendGridApiKey, _purchaseOptions.CartApi, _purchaseOptions.CartApiKey,
                _purchaseOptions.InventoryApi, _purchaseOptions.InventoryApiKey);

            return result;
        }
    }
}
