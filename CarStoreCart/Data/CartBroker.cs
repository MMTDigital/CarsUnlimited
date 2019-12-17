using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CarStoreCart.Data
{
    public class CartBroker
    {
        public CartBroker()
        {
        }

        internal string GetItemsInCart(string storageConnectionString, string sessionId)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = cloudTableClient.GetTableReference("Carts");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<CartEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            if (resultTask.Result.Result == null)
            {
                var entity = new CartEntity(sessionId);
                entity.ContentIds = "";
                entity.ContentItems = "0";

                TableOperation createOperation = TableOperation.InsertOrReplace(entity);
                table.ExecuteAsync(createOperation).Wait();

                return "0";
            }
            else
            {
                var entity = (CartEntity)resultTask.Result.Result;
                return $"{entity.ContentItems};{entity.ContentIds}";
            }
        }

        internal int ChangeItemInCart(string storageConnectionString, string sessionId, string contentItem)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = cloudTableClient.GetTableReference("Carts");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<CartEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            var cart = new CartEntity(sessionId);
            if(resultTask.Result.Result != null)
            {
                cart = (CartEntity)resultTask.Result.Result;
            }

            if(contentItem.StartsWith("-", StringComparison.InvariantCultureIgnoreCase))
            {
                contentItem = contentItem.Substring(1);
                var content = cart.ContentIds.Split(",");
                var newContent = "";

                bool found = false;

                foreach(var cartItem in content)
                {
                    if(!found)
                    {
                        if(cartItem.Equals(contentItem))
                        {
                            found = true;
                            continue;
                        }

                        newContent = $"{newContent}{cartItem},";
                    }
                }

                cart.ContentIds = newContent;
                cart.ContentItems = (int.Parse(cart.ContentItems) - 1).ToString();
            }
            else
            {
                cart.ContentIds = $"{cart.ContentIds}{contentItem},";
                cart.ContentItems = (int.Parse(cart.ContentItems) + 1).ToString();
            }

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(cart);
            table.ExecuteAsync(insertOrReplaceOperation).Wait();

            return int.Parse(cart.ContentItems);
        }

        internal void EmptyCart(string storageConnectionString, string sessionId)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = cloudTableClient.GetTableReference("Carts");
            table.CreateIfNotExistsAsync().Wait();

            TableOperation retrieveOperation = TableOperation.Retrieve<CartEntity>(sessionId, sessionId);
            var resultTask = table.ExecuteAsync(retrieveOperation);
            resultTask.Wait();

            var cart = new CartEntity(sessionId);
            if (resultTask.Result.Result != null)
            {
                cart = (CartEntity)resultTask.Result.Result;
            }

            cart.ContentIds = "";
            cart.ContentItems = "0";

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(cart);
            table.ExecuteAsync(insertOrReplaceOperation).Wait();
        }

        internal class CartEntity : TableEntity
        {
            public CartEntity()
            {

            }

            public CartEntity(string sessionId)
            {
                this.RowKey = sessionId;
                this.PartitionKey = sessionId;
                this.ContentItems = "0";
                this.ContentIds = "";
            }

            public string ContentIds { get; set; }
            public string ContentItems { get; set; }
        }
    }
}