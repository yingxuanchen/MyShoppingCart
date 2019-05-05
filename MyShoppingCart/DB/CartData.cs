using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyShoppingCart.DB
{
    public class CartData : Data
    {
        public static List<Cart> GetCartItemsByCustomerId(int customerId)
        {
            List<Cart> cartList = new List<Cart>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT ProductId, Quantity
                                FROM Cart
                                    WHERE CustomerId = '" + customerId + "'";

                SqlCommand cmd = new SqlCommand(q, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cart item = new Cart()
                    {
                        ProductId = (int)reader["ProductId"],
                        Quantity = (int)reader["Quantity"]
                    };

                    cartList.Add(item);
                }
            }

            return cartList;
        }

        public static void PopulateCartItemDetails(List<Cart> cartList)
        {
            // Get the full product list
            List<Product> fullProductList = ProductData.GetAllProducts();
            Product product;

            // iterate through cart list 
            foreach (Cart item in cartList)
            {
                // find the corresponding product in the product list
                product = fullProductList.Find(x => x.Id == item.ProductId);

                // update each cart item with product details
                item.ProductName = product.Name;
                item.ProductDescription = product.Description;
                item.UnitPrice = product.Price;
                item.TotalPrice = item.UnitPrice * item.Quantity;
            }
        }

        public static void IncrementItemInCart(int customerId, int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT * FROM Cart WHERE CustomerId =" + customerId;

                SqlDataAdapter da = new SqlDataAdapter(q, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "MyShoppingCart");

                DataTable tblCart = ds.Tables[0];

                // increase quantity of product by 1 if it existed in cart
                bool added = false;
                foreach (DataRow r in tblCart.Rows)
                    if ((int)r["ProductId"] == productId)
                    {
                        r["Quantity"] = (int)r["Quantity"] + 1;
                        added = true;
                        break;
                    }

                // add a row in the table if the product did not exist in cart
                if (!added)
                {
                    DataRow row = tblCart.NewRow();
                    row["CustomerId"] = customerId;
                    row["ProductId"] = productId;
                    row["Quantity"] = 1;
                    tblCart.Rows.Add(row);
                }

                tblCart.PrimaryKey = new DataColumn[]
                                        { tblCart.Columns["CustomerId"],
                                          tblCart.Columns["ProductId"] };

                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                da.Update(ds, "MyShoppingCart");
            }
        }

        public static void UpdateCart(int customerId, int productId, int quantity)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT * FROM Cart WHERE CustomerId =" + customerId;

                SqlDataAdapter da = new SqlDataAdapter(q, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "MyShoppingCart");

                DataTable tblCart = ds.Tables[0];
                
                // update quantity of product if it existed in cart
                bool updated = false;
                foreach (DataRow r in tblCart.Rows)
                    if ((int)r["ProductId"] == productId)
                    {
                        r["Quantity"] = quantity;
                        updated = true;
                        break;
                    }

                // add a row in the table if the product did not exist in cart
                if (!updated)
                {
                    DataRow row = tblCart.NewRow();
                    row["CustomerId"] = customerId;
                    row["ProductId"] = productId;
                    row["Quantity"] = quantity;
                    tblCart.Rows.Add(row);
                }

                tblCart.PrimaryKey = new DataColumn[]
                                        { tblCart.Columns["CustomerId"],
                                          tblCart.Columns["ProductId"] };

                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                da.Update(ds, "MyShoppingCart");
            }
        }

        public static bool IsCartEmpty(List<Cart> cartList)
        {
            if (cartList.Count == 0)
                return true;

            // cart is not empty if any item in the cart has quantity more than 0
            for (int i = 0; i < cartList.Count; i++)
                if (cartList[i].Quantity > 0)
                    return false;

            return true;
        }

        public static void ClearCart(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"DELETE FROM Cart WHERE CustomerId =" + customerId;

                SqlCommand cmd = new SqlCommand(q, conn);
                cmd.ExecuteNonQuery();                
            }
        }

    }
}