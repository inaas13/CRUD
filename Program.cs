using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CRUD_inL
{
    class Program
    {
        static void Main()
        {
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder();
                    builder.DataSource = @"5CD8222QVG\SQLEXPRESS01";
                    builder.InitialCatalog = "TelerikAcademy99";
                    builder.TrustServerCertificate = true;
                    builder.IntegratedSecurity = true;

                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        Console.WriteLine("\nQuery data example:");
                        Console.WriteLine("=========================================\n");

                        String sql = "SELECT FirstName FROM Employees";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine(reader.GetString(0));
                                }
                            }
                        }
                         AddCustomer(connection);
                         DeleteCustomerByID(connection);
                         ShowCountrySales(connection);
                        var customerId = CreateOrderAndCustomer(connection);
                          RemoveCustomer(connection, customerId);
                    }

                }
                catch (SqlException e)
                {

                    Console.WriteLine(e.ToString());
                    
                }

            }
        }
        static string AddCustomer(SqlConnection connection)
        {
            // Random användes här för att undvika dubletter 
            Random r = new Random();
            var customerId = r.Next(40, 70000);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            
            command.CommandText = $"INSERT INTO Customers (CustomerID, CompanyName, ContactName," +
                $" ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax)" +
                $" VALUES ({customerId}, 'ICA Maxi','Anders Svensson', 'Chef'," +
                $" 'Maxigatan 4','Kungälv','Västra götaland', 'Sweden','422 38'," +
                $" '0303-64329', '44467');";
            command.Connection = connection;
            command.ExecuteNonQuery();
            Console.WriteLine("Customer has successfully been added");
            return customerId.ToString(); 

        }
        static void DeleteCustomerByID(SqlConnection connection)
        {
            Console.WriteLine("Enter a Customer ID: ");
            var customerId = Console.ReadLine();
            var userFound = FindUserByID(connection, customerId);

            var orderIds = new List<int>();
            var orderDetailsIds = new List<int>();

            if (userFound)
            {
                SqlCommand command = new SqlCommand();

                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT OrderID FROM Orders WHERE CustomerID = '{customerId}'";
                command.Connection = connection;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderIds.Add(reader.GetInt32(0));
                    }
                }
                foreach (var orderId in orderIds)
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT OrderID FROM [Order Details] WHERE OrderID = '{orderId}'";
                    command.Connection = connection;
                    command.ExecuteNonQuery();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderDetailsIds.Add(reader.GetInt32(0));
                        }
                    }
                    foreach (var OrderDetailId in orderDetailsIds)
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"DELETE FROM [Order Details] WHERE OrderID = '{OrderDetailId}'";
                        command.Connection = connection;
                        command.ExecuteNonQuery();

                    }
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"DELETE FROM Orders WHERE OrderID = '{orderId}'";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    
                }


                command.CommandType = CommandType.Text;
                command.CommandText = $"DELETE FROM Customers WHERE CustomerID = '{customerId}'";
                command.Connection = connection;
                command.ExecuteNonQuery();
                Console.WriteLine("Customer has been deleted");
            }
            else
            {
                Console.WriteLine("user not found.");
            }

        }

        static void UpdateEmployee(SqlConnection connection)
        {
            Console.Write("Which user would you like to update? ");
            var employeeId = Console.ReadLine();

            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT FirstName FROM Employees WHERE EmployeeID = {employeeId}'";
            command.Connection = connection;
            var firstName = (string)command.ExecuteScalar();

            if (!string.IsNullOrEmpty(firstName))
            {
                Console.Write($"Please enter a new name for (firstName): ");
                string newName = Console.ReadLine();

                command = new SqlCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = $"UPDATE Employees SET FirstName ='{newName}' WHERE EmployeeID '{employeeId}";
                command.ExecuteNonQuery();
                Console.WriteLine("FirstName has been updated!");
            }
            else
            {
                Console.WriteLine("There is no employee with that ID!");
            }
        }

        static void ShowCountrySales(SqlConnection connection)
        {
            var dataTable = new List<EmployeeData>();
            Console.Write("Select a country: ");
            string country = Console.ReadLine(); 

            var command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT EmployeeID, OrderID FROM Orders WHERE ShipCountry = " +
                $"'{country}' GROUP BY EmployeeID, OrderID ORDER BY EmployeeID; ";
            command.Connection = connection;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var employeeData = new EmployeeData
                    {
                        EmployeeId = reader.GetInt32(0),
                        OrderId = reader.GetInt32(1)
                    };
                    dataTable.Add(employeeData);
                }
            }
            var employeeStatistics = dataTable.GroupBy(x => x.EmployeeId).ToDictionary(x => x.Key); 
            foreach (var item in employeeStatistics)
            {
                decimal totalWorth = 0;

                var orderIds = item.Value; 
                foreach (var orderid in orderIds)
                {
                    /*get the value of each order */
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT UnitPrice FROM [Order Details] WHERE OrderID = '{orderid.OrderId}'; ";
                    command.Connection = connection;

                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            totalWorth += reader.GetDecimal(0);
                        }
                    }

                }
                Console.WriteLine($"Employee with the ID: {item.Key} " +
                    $"sold a total amount of: ${totalWorth} in {country}.");
            }




        }

        static string CreateOrderAndCustomer(SqlConnection connection)
        {
          

            var customerId = AddCustomer(connection);
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Orders (CustomerID, EmployeeID, OrderDate, RequiredDate, " +
                "ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, " +
                "ShipRegion, ShipPostalCode, ShipCountry) " +
                $"VALUES ('{customerId}', '1', '20110825', '20110825'," +
                "'20110825', '3', '32', 'prrr', " +
                "'FranceWay', 'Reims', 'lol', '51100', 'France'); ";
          
            command.Connection = connection;
            command.ExecuteNonQuery();
            Console.WriteLine("Order has been added.");
            return customerId; 



        }

        static void RemoveCustomer(SqlConnection connection, string customerId)
        {
            var command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT OrderID FROM Orders WHERE CustomerID = '{customerId}'";
            command.Connection = connection;

            int orderId = 0; 

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    orderId = reader.GetInt32(0);
                }
            }
            command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"DELETE  FROM Orders WHERE OrderID = '{orderId}'";
            command.Connection = connection;
            command.ExecuteNonQuery();

            command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"DELETE  FROM Customers WHERE CustomerID = '{customerId}'";
            command.Connection = connection;
            command.ExecuteNonQuery();

            Console.WriteLine("Customer has been removed. ");

        }


        static bool FindUserByID(SqlConnection connection, string id)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT COUNT(*) FROM Customers WHERE CustomerID = '{id}'";
            command.Connection = connection;
            var found = (int)command.ExecuteScalar() == 1 ? true : false;
            return found;

        }
    }
}

