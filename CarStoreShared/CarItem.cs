using System;
namespace CarStoreShared
{
    public class CarItem
    {
        public string CarId { get; set; }
        public string CarPicture { get; set; }
        public string CarManufacturer { get; set; }
        public string CarModel { get; set; }
        public string CarInfo { get; set; }
        public double CarPrice { get; set; }
        public int CarsInStock { get; set; }

        public CarItem(string id, string pictureUri, string manufacturer, string model, string info, double price, int stock)
        {
            CarId = id;
            CarPicture = pictureUri;
            CarManufacturer = manufacturer;
            CarModel = model;
            CarInfo = info;
            CarPrice = price;
            CarsInStock = stock;
        }
    }
}
