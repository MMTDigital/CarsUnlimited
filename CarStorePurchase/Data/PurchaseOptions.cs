using System;
namespace CarStorePurchase.Data
{
    public class PurchaseOptions
    {
        public string CartApiKey { get; set; }
        public string CartApi { get; set; }
        public string InventoryApiKey { get; set; }
        public string InventoryApi { get; set; }
        public string SendGridApiKey { get; set; }
    }
}
