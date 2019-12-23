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
    public class CartController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["CartItems"] = CartProxy.GetInstance().GetCurrentNumberOfItems(HttpContext.Session.Id);

            var cartModel = new CartViewModel();
            var cartItems = CartProxy.GetInstance().GetCurrentCartItems(HttpContext.Session.Id);

            Dictionary<string, int> preDict = new Dictionary<string, int>();
            foreach (var item in cartItems)
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

            double total = 0.0;
            foreach(var key in preDict.Keys)
            {
                var car = InventoryProxy.GetInstance().GetInventoryItem(key);
                cartModel.AddCartItemToDisplay(car, preDict[key]);

                total = total + (car.CarPrice * preDict[key]);

            }

            ViewData["Total"] = string.Format("{0:N2}", total);


            return View(cartModel);
        }

        public IActionResult Remove(string carManufacturerAndId)
        {
            CartProxy.GetInstance().RemoveItem(HttpContext.Session.Id, carManufacturerAndId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Order(string email)
        {
            var cartModel = new CartViewModel();
            ViewData["CustomerEmail"] = email;
            ViewData["OrderResult"] = "Your order has been placed. An email confirmation will follow shortly.";
            ViewData["OrderBool"] = true;

            ViewData["CartItems"] = CartProxy.GetInstance().GetCurrentNumberOfItems(HttpContext.Session.Id);

            if(string.IsNullOrWhiteSpace(email))
            {
                ViewData["OrderResult"] = "Email address entered is not valid. Please try again.";
                ViewData["OrderBool"] = false;
            } else
            {
                if(!email.Contains("@") || !email.Contains("."))
                {
                    ViewData["OrderResult"] = "Email address entered is not valid. Please try again.";
                    ViewData["OrderBool"] = false;
                }
            }

            var cartItems = CartProxy.GetInstance().GetCurrentCartItems(HttpContext.Session.Id);

            Dictionary<string, int> preDict = new Dictionary<string, int>();
            foreach(var item in cartItems)
            {
                if(string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                if(preDict.ContainsKey(item))
                {
                    preDict[item]++;
                } else
                {
                    preDict.Add(item, 1);
                }
            }

            double total = 0.0;
            foreach (var key in preDict.Keys)
            {
                var car = InventoryProxy.GetInstance().GetInventoryItem(key);
                cartModel.AddCartItemToDisplay(car, preDict[key]);
                total = total + (car.CarPrice * preDict[key]);
            }

            ViewData["Total"] = string.Format("{0:N2}", total);

            if((bool)ViewData["OrderBool"] == false)
            {
                return View(cartModel);
            }

            var orderResult = PurchaseProxy.GetInstance().TryOrder(HttpContext.Session.Id, email);

            var orderArray = orderResult.Split(";");
            if(orderArray[0].Equals("Success"))
            {
                ViewData["OrderBool"] = true;
                ViewData["OrderResult"] = orderArray[1];
                ViewData["CartItems"] = "0";
            }
            else
            {
                ViewData["OrderBool"] = false;
                ViewData["OrderResult"] = orderArray[1];
            }

            return View(cartModel);
        }
    }
}
