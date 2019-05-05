using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyShoppingCart.DB
{
    public class ProductData : Data
    {
        public static List<Product> GetProductList(string searchString)
        {
            List<Product> fullProductList = GetAllProducts();
            List<Product> productList = new List<Product>(fullProductList);

            if (String.IsNullOrWhiteSpace(searchString))
                return productList;

            productList.Clear();
            searchString = searchString.ToLower();

            foreach (Product item in fullProductList)
            {
                if (item.Name.ToLower().Contains(searchString)
                    || item.Description.ToLower().Contains(searchString))
                {
                    productList.Add(item);
                }
            }

            return productList;
        }

        public static List<Product> GetAllProducts()
        {
            List<Product> productList = new List<Product>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT Id, Name, Description, Price 
                                FROM Product";

                SqlCommand cmd = new SqlCommand(q, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product item = new Product()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        Price = (decimal)reader["Price"]
                    };

                    productList.Add(item);
                }
            }

            return productList;
        }
    }
}