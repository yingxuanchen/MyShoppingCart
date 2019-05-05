using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyShoppingCart.DB
{
    public class PurchaseData : Data
    {
        public static List<Purchase> GetPurchasedItemsByCustomerId(int customerId)
        {
            List<Purchase> purchaseList = new List<Purchase>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT ProductId, ActivationCode, Date
                                FROM Purchase
                                    WHERE CustomerId =" + customerId;

                SqlCommand cmd = new SqlCommand(q, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int productId = (int)reader["ProductId"];
                    string activationCode = (string)reader["ActivationCode"];
                    DateTime datePurchased = (DateTime)reader["Date"];

                    bool added = false;

                    // add activation code into existing purchase if it is same product bought on the same day
                    for (int i=0; i<purchaseList.Count; i++)
                        if (purchaseList[i].ProductId==productId && purchaseList[i].DatePurchased==datePurchased)
                        {
                            purchaseList[i].ActivationCodes.Add(activationCode);
                            purchaseList[i].Quantity++;
                            added = true;
                            break;
                        }

                    // add new line in purchase list if did not exist
                    if (!added)
                    {
                        Purchase item = new Purchase()
                        {
                            ProductId = productId,
                            DatePurchased = datePurchased,
                            Quantity = 1,
                            ActivationCodes = new List<string>() { activationCode }
                        };
                        purchaseList.Add(item);
                    }                    
                }
            }

            return purchaseList;
        }

        public static void PopulatePurchasedItemDetails(List<Purchase> purchaseList)
        {
            // Get the full product list
            List<Product> fullProductList = ProductData.GetAllProducts();
            Product product;

            // iterate through cart list 
            foreach (Purchase item in purchaseList)
            {
                // find the corresponding product in the product list
                product = fullProductList.Find(x => x.Id == item.ProductId);

                // update each purchased item with product details
                item.ProductName = product.Name;
                item.ProductDescription = product.Description;
            }
        }

        public static void Checkout(int customerId, List<Cart> cartList)
        {
            // add cart items into database purchase table
            UpdatePurchases(customerId, cartList);

            // clear cart of all items
            CartData.ClearCart(customerId);
        }

        private static void UpdatePurchases(int customerId, List<Cart> cartList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT * FROM Purchase WHERE CustomerId =" + customerId;

                SqlDataAdapter da = new SqlDataAdapter(q, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "MyShoppingCart");

                DataTable tblPurchases = ds.Tables[0];

                DateTime dateToday = DateTime.Today;

                for (int i=0; i<cartList.Count; i++)
                {
                    while (cartList[i].Quantity>0)
                    {
                        // generate new activation code
                        string activationCode = Guid.NewGuid().ToString();
                        
                        // add row into purchase table
                        DataRow row = tblPurchases.NewRow();
                        row["CustomerId"] = customerId;
                        row["ProductId"] = cartList[i].ProductId;
                        row["ActivationCode"] = activationCode;
                        row["Date"] = dateToday;
                        tblPurchases.Rows.Add(row);

                        cartList[i].Quantity--;
                    }
                }
                
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                da.Update(ds, "MyShoppingCart");
            }
        }
    }
}