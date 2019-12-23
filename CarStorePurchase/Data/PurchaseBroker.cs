using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using CarStoreShared;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;

namespace CarStorePurchase.Data
{
    public class PurchaseBroker
    {
        public PurchaseBroker()
        {
        }

        internal string CheckoutCart(string sessionId, string customerEmail, string sendGridKey, string cartEndpoint, string cartKey, string inventoryEndpoint, string inventoryKey)
        {
            if(!cartEndpoint.EndsWith("/", StringComparison.InvariantCulture))
            {
                cartEndpoint = $"{cartEndpoint}/";
            }

            if(!inventoryEndpoint.EndsWith("/", StringComparison.InvariantCulture))
            {
                inventoryEndpoint = $"{inventoryEndpoint}/";
            }

            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new ApiPackage { SessionIdentifier = sessionId, ApiKey = cartKey });
            var externalTask = client.PostAsync($"{cartEndpoint}api/cart", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();
            var returnedValue = returnedValueTask.Result;

            var cartItems = returnedValue.Split(";")[1].Split(",");

            var preDict = new Dictionary<string, int>();

            foreach(var item in cartItems)
            {
                if(string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                if(preDict.ContainsKey(item))
                {
                    preDict[item]++;
                }
                else
                {
                    preDict.Add(item, 1);
                }
            }

            if(preDict.Keys.Count < 1)
            {
                return "Failed;No items in cart";
            }

            double total = 0.0;
            Dictionary<CarItem, int> carDict = new Dictionary<CarItem, int>();

            foreach (var key in preDict.Keys)
            {
                var car = this.GetInventoryItem(key, inventoryEndpoint, inventoryKey);
                if(car.CarsInStock < preDict[key])
                {
                    return $"Failed;There are only {car.CarsInStock} {car.CarManufacturer} {car.CarModel} available. Please adjust your order.";
                }
                carDict.Add(car, preDict[key]);
                total += (car.CarPrice * preDict[key]);
            }

            var instructionString = "";
            foreach (var key in carDict.Keys)
            {
                instructionString += $"{key.CarManufacturer}:{key.CarId}:-{carDict[key]};";
            }

            content = JsonConvert.SerializeObject(new ApiPackage { ApiKey = inventoryKey, SessionIdentifier = sessionId, ContentItem = instructionString });
            externalTask = client.PutAsync($"{inventoryEndpoint}api/inventory", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();
            returnedValueTask.Wait();

            returnedValue = returnedValueTask.Result;

            if(!returnedValue.Equals("Done!"))
            {
                return "Failed;An error occured adjusting stock levels, please try again";
            }

            externalTask = client.DeleteAsync($"{cartEndpoint}api/cart/{sessionId}");
            externalTask.Wait();

            var sendGridClient = new SendGridClient(sendGridKey);
            var formattedMessage = "<h3>Thank you for your order at Cars Unlimited!</h3><p>You ordered the following vehicles:</p>";
            var unformattedMessage = "Thank you for your order at Cars Unlimited! \r\n You ordered the following vehicles: \r\n \r\n";

            foreach(var car in carDict)
            {
                formattedMessage += $"<p>{car.Value}x <b>{car.Key.CarManufacturer} {car.Key.CarModel}</b> at <i>&#163; {string.Format("{0:N2}", car.Key.CarPrice)} each, totalling <b>&#163; {string.Format("{0:N2}", (car.Key.CarPrice * car.Value))}</b></p>";
                unformattedMessage += $"{car.Value}x {car.Key.CarManufacturer} {car.Key.CarModel} at GBP {string.Format("{0:N2}", car.Key.CarPrice)} each, totalling GBP {string.Format("{0:N2}", (car.Key.CarPrice * car.Value))} \r\n";
            }

            formattedMessage += $"<h3>Total: &#163; {string.Format("{0:N2}", total)}</h3>";
            unformattedMessage += $"\r\n \r\n Total: GBP {string.Format("{0:N2}", total)}";

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("orders@carsunlimited.online", "Cars Unlimited Team"),
                Subject = "Thank you for your order at Cars Unlimited",
                PlainTextContent = unformattedMessage,
                HtmlContent = formattedMessage
            };
            msg.AddTo(new EmailAddress(customerEmail));
            var responseTask = sendGridClient.SendEmailAsync(msg);

            try
            {
                responseTask.Wait();
            }
            catch
            {
                return "Success;Thank you for your order! Unforunately we are currently unable to confirm the order via e-mail. Your order has been successful.";
            }

            return "Success;Thank you for your order! You should receive a confirmation e-mail soon.";
        }

        private CarItem GetInventoryItem(string key, string inventoryApi, string inventoryKey)
        {
            HttpClient client = new HttpClient();

            var content = JsonConvert.SerializeObject(new ApiPackage { ApiKey = inventoryKey });
            var externalTask = client.PostAsync($"{inventoryApi}api/inventory/{key}", new StringContent(content, Encoding.UTF8, "application/json"));
            externalTask.Wait();

            var returnedValueTask = externalTask.Result.Content.ReadAsStringAsync();

            returnedValueTask.Wait();

            var returnedValue = returnedValueTask.Result;

            return JsonConvert.DeserializeObject<CarItem>(returnedValue);
        }
    }
}
