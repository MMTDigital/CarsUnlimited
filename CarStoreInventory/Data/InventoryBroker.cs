using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarStoreShared;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CarStoreInventory.Data
{
    public class InventoryBroker
    {
        private Dictionary<string, CarItem> carsInStock;
        private bool hasInitialized = false;

        public InventoryBroker()
        {
        }

        public async Task PullAsync(string storageConnectionString)
        {
            CloudStorageAccount storage = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("inventory");
            await table.CreateIfNotExistsAsync();

            TableQuery<CarEntity> query = new TableQuery<CarEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, null));
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            Dictionary<string, CarItem> carsInStorageAccount = new Dictionary<string, CarItem>();
            foreach(var result in queryResult)
            {
                carsInStorageAccount.Add($"{result.PartitionKey}:{result.RowKey}",
                    new CarItem(result.RowKey, result.CarPicture, result.PartitionKey,
                    result.CarModel, result.CarInfo, result.CarPrice, result.CarsInStock));
            }

            carsInStock = carsInStorageAccount;

            hasInitialized = true;
        }

        
    }

    public class CarEntity : TableEntity
    {
        public CarEntity()
        {

        }

        public CarEntity(CarItem item)
        {
            this.PartitionKey = item.CarManufacturer;
            this.RowKey = item.CarId;
            this.CarModel = item.CarModel;
            this.CarInfo = item.CarInfo;
            this.CarPicture = item.CarPicture;
            this.CarsInStock = item.CarsInStock;
            this.CarPrice = item.CarPrice;
        }

        public string CarModel { get; set; }
        public double CarPrice { get; set; }
        public int CarsInStock { get; set; }
        public string CarPicture { get; set; }
        public string CarInfo { get; set; }
    }
}
