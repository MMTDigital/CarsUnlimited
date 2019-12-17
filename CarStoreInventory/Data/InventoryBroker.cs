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
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = cloudTableClient.GetTableReference("inventory");
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

        public async Task UpdateEntityAsync(CarItem changedItem, string storageConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = cloudTableClient.GetTableReference("inventory");
            await table.CreateIfNotExistsAsync();

            var operation = TableOperation.Retrieve<CarEntity>(changedItem.CarManufacturer, changedItem.CarId);
            var result = await table.ExecuteAsync(operation);
            var retrievedEntity = (CarEntity)result.Result;

            retrievedEntity.CarInfo = changedItem.CarInfo;
            retrievedEntity.CarPrice = changedItem.CarPrice;
            retrievedEntity.CarPicture = changedItem.CarPicture;
            retrievedEntity.CarModel = changedItem.CarModel;
            retrievedEntity.CarsInStock = changedItem.CarsInStock;

            operation = TableOperation.Replace(retrievedEntity);

            await table.ExecuteAsync(operation);
        }

        internal string ChangeStock(string contentItem, string storageConnectionString)
        {
            if(!hasInitialized)
            {
                throw new InvalidOperationException("Inventory is not initialized.");
            }

            if(string.IsNullOrWhiteSpace(contentItem))
            {
                throw new InvalidOperationException("Instruction string has not been provided");
            }

            if(!contentItem.Contains(";"))
            {
                throw new InvalidOperationException("String is in an invalid format");
            }

            var contentArray = contentItem.Split(';');
            var changesToMake = new List<Task>();

            foreach (var stockItem in contentArray)
            {
                var stockArray = stockItem.Split(':');

                if (stockArray.Length != 3)
                {
                    continue;
                }

                var carManufacturer = stockArray[0];
                var carId = stockArray[1];
                var carStockChange = int.Parse(stockArray[2]);

                var carItem = GetInventoryItemById($"{stockArray[0]}:{stockArray[1]}");
                carItem.CarsInStock += carStockChange;
                var t = UpdateEntityAsync(carItem, storageConnectionString);
                changesToMake.Add(t);
            }

            Task.WaitAll(changesToMake.ToArray());
            return "Done!";
        }

        internal CarItem GetInventoryItemById(string id)
        {
            if(!hasInitialized)
            {
                throw new InvalidOperationException("Inventory is not initialized");
            }

            return carsInStock[id];
        }

        public Dictionary<string, CarItem> GetCurrentInventory()
        {
            if(hasInitialized)
            {
                return carsInStock;
            }

            throw new InvalidOperationException("Inventory is not initialized");
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
