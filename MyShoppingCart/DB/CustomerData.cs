using MyShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyShoppingCart.DB
{
    public class CustomerData : Data
    {
        public static Customer GetCustomerByUsername(string username)
        {
            Customer customer = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT Id, Username, Password
                                FROM Customer
                                    WHERE Username = '" + username + "'";

                SqlCommand cmd = new SqlCommand(q, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    customer = new Customer()
                    {
                        Id = (int)reader["Id"],
                        Username = (string)reader["Username"],
                        Password = (string)reader["Password"]
                    };
                }
            }

            return customer;
        }


        public static Customer GetCustomerBySessionId(string sessionId)
        {
            Customer customer = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = @"SELECT Id, FirstName, LastName
                                FROM Customer
                                    WHERE SessionId = '" + sessionId + "'";

                SqlCommand cmd = new SqlCommand(q, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    customer = new Customer()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                }
            }

            return customer;
        }
    }
}