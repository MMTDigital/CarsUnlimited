using System;
namespace CarStoreShared
{
    public class CarItem
    {
        public string CarId;
        public string CarPicture;
        public string CarManufacturer;
        public string CarModel;
        public string CarInfo;
        public double CarPrice;
        public int CarsInStock;

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
