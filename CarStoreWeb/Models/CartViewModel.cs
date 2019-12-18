using System;
using System.Collections.Generic;
using CarStoreShared;

namespace CarStoreWeb.Models
{
    public class CartViewModel
    {
        private Dictionary<CarItem, int> _cartItemsToDisplay = new Dictionary<CarItem, int>();

        public void AddCartItemToDisplay(CarItem carItem, int amount)
        {
            if(_cartItemsToDisplay == null)
            {
                _cartItemsToDisplay = new Dictionary<CarItem, int>();
            }

            _cartItemsToDisplay.Add(carItem, amount);
        }

        public Dictionary<CarItem, int> CartItemsToDisplay { get { return _cartItemsToDisplay; } }
    }
}
