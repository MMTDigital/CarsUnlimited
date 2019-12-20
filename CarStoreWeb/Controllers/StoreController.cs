using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Data;
using CarStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarStoreWeb.Controllers
{
    public class StoreController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["CartItems"] = CartProxy.GetInstance().GetCurrentNumberOfItems(HttpContext.Session.Id);
            var inventory = InventoryProxy.GetInstance().GetInventory();

            var storeModel = new StoreViewModel();

            foreach(var key in inventory.Keys)
            {
                storeModel.AddCarToDisplay(inventory[key]);
            }

            return View(storeModel);
        }

        public string AddToCart(string manufacturerAndIdString)
        {
            if(string.IsNullOrWhiteSpace(manufacturerAndIdString))
            {
                throw new InvalidOperationException();
            }

            if(!manufacturerAndIdString.Contains(":"))
            {
                throw new InvalidOperationException();
            }

            var manufacturerAndIdArray = manufacturerAndIdString.Split(":");
            if(manufacturerAndIdArray.Length != 2)
            {
                throw new InvalidOperationException();
            }

            return CartProxy.GetInstance().AddItem(HttpContext.Session.Id, manufacturerAndIdString).ToString();
        }
    }
}
