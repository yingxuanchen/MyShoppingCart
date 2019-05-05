using MyShoppingCart.DB;
using MyShoppingCart.Filters;
using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShoppingCart.Controllers
{
    [AuthorizeFilter]
    public class PurchaseController : Controller
    {
        public ActionResult Checkout(string sessionId)
        {
            Customer customer = CustomerData.GetCustomerBySessionId(sessionId);
            List<Cart> cartList = CartData.GetCartItemsByCustomerId(customer.Id);

            // display alert in browser if cart is empty
            if (CartData.IsCartEmpty(cartList))
            {
                TempData["msg"] = "<script>alert('No item in cart!');</script>";
                return RedirectToAction("ViewCart","Cart", new { sessionId });
            }

            // if cart has something, check out and go to view purchase page
            PurchaseData.Checkout(customer.Id, cartList);
            return RedirectToAction("ViewPurchases", new { sessionId });
        }

        public ActionResult ViewPurchases(string sessionId)
        {
            Customer customer = CustomerData.GetCustomerBySessionId(sessionId);
            List<Purchase> purchaseList = PurchaseData.GetPurchasedItemsByCustomerId(customer.Id);
            PurchaseData.PopulatePurchasedItemDetails(purchaseList);

            ViewData["sessionId"] = sessionId;
            ViewData["purchaseList"] = purchaseList;

            return View();
        }
    }
}