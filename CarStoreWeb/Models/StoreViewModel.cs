using System;
using System.Collections.Generic;
using CarStoreShared;

namespace CarStoreWeb.Models
{
    public class StoreViewModel
    {
        private List<CarItem> _carsToDisplay = new List<CarItem>();

        public void AddCarToDisplay(CarItem carItem)
        {
            if(_carsToDisplay == null)
            {
                _carsToDisplay = new List<CarItem>();
            }

            _carsToDisplay.Add(carItem);
        }

        public List<CarItem> CarsToDisplay { get { return _carsToDisplay; } }
    }
}
