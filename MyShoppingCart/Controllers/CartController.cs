using MyShoppingCart.DB;
using MyShoppingCart.Filters;
using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShoppingCart.Controllers
{
    public class CartController : Controller
    {
        [AuthorizeFilter]
        public ActionResult ViewCart(string sessionId, int? productId, int? quantity)
        {
            Customer customer = CustomerData.GetCustomerBySessionId(sessionId);

            // update cart if the quantity was changed in the cart
            if (productId != null && quantity != null)
                CartData.UpdateCart(customer.Id, (int)productId, (int)quantity);

            List<Cart> cartList = CartData.GetCartItemsByCustomerId(customer.Id);
            CartData.PopulateCartItemDetails(cartList);

            ViewData["sessionId"] = sessionId;
            ViewData["cartList"] = cartList;

            return View();
        }
    }
}