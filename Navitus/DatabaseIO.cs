using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Navitus
{

    // Working with SQLite database
    public static class DatabaseIO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); // log4net

        // Getting all customers as ObjectList<CustomerObj> object
        public static async Task<ObjectList<CustomerObj>> GetAllCustomers()
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Getting all customers");
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "SELECT * FROM Customers";
                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    DbDataReader result = await command.ExecuteReaderAsync();
                    // Processing received data from database
                    ObjectList<CustomerObj> customers = new ObjectList<CustomerObj>();

                    while (await result.ReadAsync())
                    {
                        customers.List.Add(new CustomerObj(result.GetString(1), result.GetInt32(2), result.GetString(3), result.GetInt32(0)));
                    }

                    db.Close();
                    log.Info("COMPLETED: Received all customers, count: " + customers.Count);
                    return customers;

                }
            }
            catch (Exception e)
            {
                log.Error("ERROR: Exception on GetAllCustomers(), message: " + e.Message);
                return null;
            }
        }

        // Getting specific customers from database based on search queries
        public static async Task<ObjectList<CustomerObj>> GetCustomer(CustomerObj customer)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Getting customers based on customer: " + JsonConvert.SerializeObject(customer));
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "SELECT * FROM Customers WHERE";
                    if (customer.Id > 0) SQLString += " Id = @id AND";
                    if (customer.Name.Length > 0) SQLString += " Name = @name AND";
                    if (customer.Age >= 0) SQLString += " Age = @age  AND";
                    if (customer.Comment.Length > 0) SQLString += " Comment = @comment  AND";
                    if (SQLString.Substring(SQLString.Length - 3) == "AND") SQLString = SQLString.Remove(SQLString.Length - 3);

                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@id", customer.Id);
                    command.Parameters.AddWithValue("@name", customer.Name);
                    command.Parameters.AddWithValue("@age", customer.Age);
                    command.Parameters.AddWithValue("@comment", customer.Comment);
                    DbDataReader result = await command.ExecuteReaderAsync();
                    // Processing received data from database
                    ObjectList<CustomerObj> customers = new ObjectList<CustomerObj>();

                    while (await result.ReadAsync())
                    {
                        customers.List.Add(new CustomerObj(result.GetString(1), result.GetInt32(2), result.GetString(3), result.GetInt32(0)));
                    }
                    db.Close();
                    log.Info("COMPLETED: Received customers based on search results, count: " + customers.Count);
                    return customers;

                }
            }


            catch (Exception e)
            {
                log.Error("ERROR: Exception on GetCustomer(), message: " + e.Message);
                return null;
            }
        }


        // Adding new customer into database
        public static async Task<bool> AddCustomer(CustomerObj customer)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Adding customer: " + JsonConvert.SerializeObject(customer));
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "INSERT INTO Customers (Name, Age, Comment) VALUES ( @name, @age, @comment ); SELECT last_insert_rowid(); COMMIT;";

                    if (customer.Name.Length == 0)
                    {
                        log.Info("FAILED: Name length must be greater than 0: " + JsonConvert.SerializeObject(customer));
                        return false;
                    }

                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@name", customer.Name);

                    if (customer.Age >= 0)
                    {
                        command.Parameters.AddWithValue("@age", customer.Age);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@age", "NULL");
                    }

                    command.Parameters.AddWithValue("@comment", customer.Comment);


                    DbDataReader result = await command.ExecuteReaderAsync();
                    // Processing received data from database
                    await result.ReadAsync();
                    int id = result.GetInt32(0);
                    db.Close();


                    if (id > 0)
                    {
                        log.Info("COMPLETED: Added customer " + JsonConvert.SerializeObject(customer) + " , result: " + id);
                        await AddHistory(id, "Registered");
                        return true;
                    }
                    else
                    {
                        log.Warn("WARN: Customer not added, details: " + JsonConvert.SerializeObject(customer) + " , result: " + id);
                        return false;
                    }


                }
            }
            catch (Exception e)
            {
                log.Error("ERROR: Exception on GetCustomer(), message: " + e.Message);
                return false;
            }

        }


        // Updating specific customer on database based on its id
        public static async Task<bool> UpdateCustomer(CustomerObj customer)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Updating customer: " + JsonConvert.SerializeObject(customer));
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "UPDATE Customers SET Name = @name, Age = @age, Comment = @comment WHERE Id = @id";

                    if (customer.Name.Length == 0)
                    {
                        log.Info("FAILED: Name length must be greater than 0: " + JsonConvert.SerializeObject(customer));
                        return false;
                    }

                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@id", customer.Id);
                    command.Parameters.AddWithValue("@name", customer.Name);

                    if (customer.Age >= 0)
                    {
                        command.Parameters.AddWithValue("@age", customer.Age);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@age", "NULL");
                    }

                    command.Parameters.AddWithValue("@comment", customer.Comment);


                    int result = await command.ExecuteNonQueryAsync();
                    // Processing received data from database
                    db.Close();
                    if (result > 0)
                    {
                        log.Info("COMPLETED: Updated customer " + JsonConvert.SerializeObject(customer) + " , result: " + result);
                        await AddHistory(customer.Id, "Updated");
                        return true;
                    }
                    else
                    {
                        log.Warn("WARN: Customer failed to update: " + JsonConvert.SerializeObject(customer) + " , result: " + result);
                        return false;
                    }






                }
            }


            catch (Exception e)
            {
                log.Error("ERROR: Exception on UpdateCustomer(), message: " + e.Message);
                return false;
            }
        }

        // Deleting customer from database
        public static async Task<bool> DeleteCustomer(int Id)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Deleting customer if Id: " + Id);
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "DELETE FROM Customers WHERE Id = @id";
                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@id", Id);
                    int result = await command.ExecuteNonQueryAsync();
                    // Processing received data from database
                    db.Close();
                    if (result > 0)
                    {
                        log.Info("COMPLETED: Deleted customer of Id: " + Id);
                        await AddHistory(Id, "Deleted");
                        return true;
                    }
                    else
                    {
                        log.Warn("WARN: Customer of Id: " + Id + " failed to delete or does not exist.");
                        return false;
                    }


                }
            }


            catch (Exception e)
            {
                log.Error("ERROR: Exception on DeleteCustomer(), message: " + e.Message);
                return false;
            }
        }


        // Adding entry into history, this function integrated into other DatabaseIO class methods
        public static async Task AddHistory(int id, string text)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Adding into history event, customer ID: " + id + " , event: " + text);
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "INSERT INTO History (CustomerID, Status) VALUES (@id, @event);";
                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@event", text);
                    int result = await command.ExecuteNonQueryAsync();
                    db.Close();
                    // Processing received data from database
                    if (result > 0)
                    {
                        log.Info("COMPLETED: Customer event added into database, customer ID: " + id + " , event: " + text);
                    }
                    else
                    {
                        log.Warn("WARN: Customer event not added into database, customer ID: " + id + " , event: " + text);
                    }

                }
            }
            catch (Exception e)
            {
                log.Error("ERROR: Exception on AddHistory(), message: " + e.Message);
            }
        }

        // Finding all history of of the specific user id
        public static async Task<ObjectList<string>> ShowHistory(int id)
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection(LoadConnectionString()))
                {
                    log.Info("STARTING: Finding all customer history, customer ID: " + id);
                    db.Open();
                    // Constructing SQL command and executing it
                    string SQLString = "SELECT Status FROM History WHERE CustomerID = @id";
                    SQLiteCommand command = new SQLiteCommand(SQLString, db);
                    command.Parameters.AddWithValue("@id", id);
                    DbDataReader result = await command.ExecuteReaderAsync();
                    ObjectList<string> history = new ObjectList<string>();
                    // Processing received data from database
                    while (await result.ReadAsync())
                    {
                        history.List.Add(result.GetString(0));
                    }

                    db.Close();


                    if (history.List.Count > 0)
                    {
                        log.Info("COMPLETED: Customer history received ID: " + id);
                        return history;
                    }
                    else
                    {
                        log.Warn("COMPLETED: Customer data does not exists ID: " + id);
                        return null;
                    }

                }
            }
            catch (Exception e)
            {
                log.Error("ERROR: Exception on AddHistory(), message: " + e.Message);
                return null;
            }
        }


        // Getting database connection string from  App.config
        public static string LoadConnectionString(string id = "SQLite_database")
        {

            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
