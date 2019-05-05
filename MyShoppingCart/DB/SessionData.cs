using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyShoppingCart.DB
{
    public class SessionData : Data
    {
        public static string CreateSession(int customerId)
        {
            string sessionId = Guid.NewGuid().ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string q = @"UPDATE Customer 
                                SET SessionId = '" + sessionId + "'" 
                                    + " WHERE Id =" + customerId;
                SqlCommand cmd = new SqlCommand(q, conn);
                cmd.ExecuteNonQuery();
            }

            return sessionId;
        }

        public static bool IsActiveSessionId(string sessionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string q = @"SELECT COUNT(*) FROM Customer
                                WHERE sessionId = '" + sessionId + "'";
                SqlCommand cmd = new SqlCommand(q, conn);
                int count = (int)cmd.ExecuteScalar();
                return (count == 1);
            }
        }

        public static void RemoveSession(string sessionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string q = @"UPDATE Customer SET SessionId = NULL 
                                WHERE SessionId = '" + sessionId + "'";
                SqlCommand cmd = new SqlCommand(q, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}