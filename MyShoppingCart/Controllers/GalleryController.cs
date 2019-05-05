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
    public class GalleryController : Controller
    {
        [AuthorizeFilter]
        public ActionResult ViewGallery(string sessionId, string searchString, int? productId)
        {
            Customer customer = CustomerData.GetCustomerBySessionId(sessionId);
            List<Product> productList = ProductData.GetProductList(searchString);

            if (productId != null)
                CartData.IncrementItemInCart(customer.Id, (int)productId);

            List<Cart> cartList = CartData.GetCartItemsByCustomerId(customer.Id);
            
            ViewData["sessionId"] = sessionId;
            ViewData["customer"] = customer;
            ViewData["cartList"] = cartList;
            ViewData["productList"] = productList;
            ViewData["searchString"] = searchString;

            return View();
        }
    }
}