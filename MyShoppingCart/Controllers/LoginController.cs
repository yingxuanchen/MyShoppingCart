using MyShoppingCart.DB;
using MyShoppingCart.Models;
using MyShoppingCart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShoppingCart.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(string username, string password)
        {
            if (username == null)
                return View();

            Customer customer = CustomerData.GetCustomerByUsername(username);
            if (customer == null || !Hash.CheckPassword(password, customer))
                return View();

            string sessionId = SessionData.CreateSession(customer.Id);
            return RedirectToAction("ViewGallery", "Gallery", new { sessionId });
        }
    }
}