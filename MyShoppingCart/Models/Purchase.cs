using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShoppingCart.Models
{
    public class Purchase
    {
        //public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public List<string> ActivationCodes { get; set; }
        public DateTime DatePurchased { get; set; }

        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }
}