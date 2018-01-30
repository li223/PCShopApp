using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace PCShopApp.Objects
{
    public class DatabaseConfig
    {
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("database")]
        public string Database { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }

        public static DatabaseConfig LoadConfig()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "dbconfig.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(new DatabaseConfig(), Formatting.Indented));
                MessageBox.Show("Config was not found and a new one has been created. Exiting program.");
                Environment.Exit(0);
            }
            var data = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DatabaseConfig>(data);
        }
    }

    public class Login
    {
        [JsonProperty("StaffId")]
        public int StaffId { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
    }
    public class Staff
    {
        [JsonProperty("StaffId")]
        public int Id { get; set; }
        [JsonProperty("StaffFname")]
        public string FirstName { get; set; }
        [JsonProperty("StaffSname")]
        public string Surname { get; set; }
    }
    public class Item
    {
        [JsonProperty("ItemName")]
        public string Name { get; set; }
        [JsonProperty("ItemPrice")]
        public decimal Price { get; set; }
    }
    public class Order
    {
        [JsonProperty("OrderId")]
        public int Id { get; set; }
        [JsonProperty("CustomerId")]
        public int CustomerId { get; set; }
        [JsonProperty("StaffId")]
        public int StaffId { get; set; }
        [JsonProperty("PaymentType")]
        public string PaymentType { get; set; }
        [JsonProperty("Parts")]
        public string Parts { get; set; }
        [JsonProperty("Total")]
        public decimal Total { get; set; }
    }
    public class Customer
    {
        [JsonProperty("CustomerId")]
        public int Id { get; set; }
        [JsonProperty("fname")]
        public string FirstName { get; set; }
        [JsonProperty("sname")]
        public string Surname { get; set; }
        [JsonProperty("telephone")]
        public ulong Phone { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
