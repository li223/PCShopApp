using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCShopApp
{
    public class DatabaseCommands
    {
        public MySqlConnection CreateConnection(string database, string server, string password, string user)
        {
            var conn = new MySqlConnection($"SERVER={server}; DATABASE={database}; UID={user}; PASSWORD={password};");
            return conn;
        }
        public async Task<int> CreateAsync(MySqlConnection connextion, string entity, string name)
        {
            var cmd = new MySqlCommand($"CREATE {entity} {name};");
            var rowchanged = await cmd.ExecuteNonQueryAsync();
            return rowchanged;
        }
        public async Task<IEnumerable<DBEntityObject>> SelectAsync(MySqlConnection connection, string table, string[] columns = null, DBEntityObject where = null)
        {
            string query = $"SELECT * FROM {table}";
            if (where != null) query += $" WHERE {where.Column} = {where.Value}";
            var cmd = new MySqlCommand($"{query};", connection);
            var reader = await cmd.ExecuteReaderAsync();
            List<DBEntityObject> objects = new List<DBEntityObject>();
            int count = 0;
            while (await reader.ReadAsync())
            {
                objects.Add(new DBEntityObject()
                {
                    Column = reader.GetName(count),
                    Value = reader.GetString(count)
                });
                count++;
            }
            reader.Close();
            return objects;
        }
        public async Task<int> UpdateAsync(MySqlConnection connection,  string table, DBEntityObject[] entries)
        {
            var cmd = new MySqlCommand($"UPDATE {table} SET {string.Join(", ", entries.Select(x => $"{x.Column} = '{x.Value}'"))};", connection);
            var rowchanged = await cmd.ExecuteNonQueryAsync();
            return rowchanged;
        }
        public async Task<int> InsertAsync(MySqlConnection connection,  string table, DBEntityObject[] entries)
        {
            var cmd = new MySqlCommand($"INSERT INTO {table} VALUES ({string.Join(", ", entries.Select(x => x.Value))});", connection);
            var rowchanged = await cmd.ExecuteNonQueryAsync();
            return rowchanged;
        }
        public async Task<int> DeleteAsync(MySqlConnection connection, string table, DBEntityObject[] entries)
        {
            var cmd = new MySqlCommand($"DELETE FROM {table} WHERE {string.Join(", ", entries.Select(x => $"{x.Column} = '{x.Value}'"))};", connection);
            var rowchanged = await cmd.ExecuteNonQueryAsync();
            return rowchanged;
        }
    }
    public static class DatabaseCommandsExtensions
    {
        public static async Task OpenConnectionAsync(this MySqlConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
            await connection.OpenAsync();
        }
    }
    public sealed class DBEntityObject
    {
        public string Column { get; set; }
        public string Value { get; set; }
    }
}
