﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OrdersApp
{
    class Program
    {
        private static string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Database=orderssystem";

        static void Main(string[] args)
        {
            //string command = args[0];
            string command = Console.ReadLine();

            if (command == "readorder")
            {
                List<Order> orders = ReadOrders();
                foreach (Order order in orders)
                {
                    Console.WriteLine(order.ProductName);
                }
            }
            else if (command == "insert")
            {
                int createdOrderId = InsertOrder(4, "PRODUCTNAME", 1000);
                Console.WriteLine("Created order: " + createdOrderId);
            }
            else if (command == "update")
            {
                UpdateOrder(1, "UPDATED PRODUCTNAME");
            }
            else if (command == "stats")
            {
                List<Order> orders = ReadOrders();
                List<CustomerStatistics> customers = new List<CustomerStatistics>();
                foreach (Order order in orders)
                {
                    CustomerStatistics customer = customers.Find(c => c.customerId == order.CustomerId);
                    if (customer == null)
                    {
                        customer = new CustomerStatistics();
                        customer.customerId = order.CustomerId;
                        customer.ordersCount = 0;
                        customer.totalCost = 0;
                        customers.Add(customer);
                    }
                    customer.ordersCount++;
                    customer.totalCost += order.Price;
                }
                foreach (CustomerStatistics customer in customers)
                {
                    Console.WriteLine(customer.ToString());
                }
            }

            Console.ReadKey();
        }

        private static List<Order> ReadOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT
                            [OrderId],
                            [ProductName],
                            [Price],
                            [CustomerId]
                        FROM [Order]";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                ProductName = Convert.ToString(reader["ProductName"]),
                                Price = Convert.ToInt32(reader["Price"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"])
                            };
                            orders.Add(order);
                        }
                    }
                }
            }
            return orders;
        }

        private static int InsertOrder(int customerId, string productName, int price)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO [Order]
                       ([ProductName],
                        [Price],
                        [CustomerId]) 
                    VALUES 
                       (@productName,
                        @price,
                        @customerId)
                    SELECT SCOPE_IDENTITY()";

                    cmd.Parameters.Add("@productName", SqlDbType.NVarChar).Value = productName;
                    cmd.Parameters.Add("@price", SqlDbType.Int).Value = price;
                    cmd.Parameters.Add("@customerId", SqlDbType.Int).Value = customerId;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private static void UpdateOrder(int orderId, string productName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE [Order]
                        SET [ProductName] = @productName
                        WHERE OrderId = @orderId";

                    command.Parameters.Add("@orderId", SqlDbType.BigInt).Value = orderId;
                    command.Parameters.Add("@productName", SqlDbType.NVarChar).Value = productName;

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
