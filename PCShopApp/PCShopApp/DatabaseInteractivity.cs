using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

public class DatabaseInteractivity
{
    public NpgsqlConnection CreateConnection(string database, string host, string password, string user, int port = 5432)
    {
        return new NpgsqlConnection($"Host={host}; Database={database}; Username={user}; Password={password}; Port={port}");
    }
    public async Task<IEnumerable<T>> SelectAsync<T>(string table, NpgsqlConnection connection)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"SELECT * FROM {table};", connection);
        var reader = await cmd.ExecuteReaderAsync();
        var data = new List<Dictionary<string, string>>();
        while (await reader.ReadAsync())
        {
            if (!reader.HasRows) break;
            var parent_child = new Dictionary<string, string>();
            for (int count = 0; count != reader.FieldCount; count++)
                parent_child.Add(reader.GetName(count), reader.GetValue(count).ToString());
            data.Add(parent_child);
        }
        reader.Close();
        var jsonobj = JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(data));
        connection.Close();
        return jsonobj;
    }
    public async Task<int> UpdateAsync(NpgsqlConnection connection, string table, IEnumerable<DBEntityObject> entries)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"UPDATE {table} SET {string.Join(", ", entries.Select(x => $"{x.Column} = '{x.Value}'"))};", connection);
        var rowchanged = await cmd.ExecuteNonQueryAsync();
        connection.Close();
        return rowchanged;
    }
    public async Task<int> UpdateAsync(NpgsqlConnection connection, string table, DBEntityObject entry)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"UPDATE {table} SET {entry.Column} = '{entry.Value}'", connection);
        var rowchanged = await cmd.ExecuteNonQueryAsync();
        connection.Close();
        return rowchanged;
    }
    public async Task<int> InsertAsync(NpgsqlConnection connection, string table, string[] entries)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"INSERT INTO {table} VALUES ({string.Join(", ", entries.Select(x => $"'{x}'"))});", connection);
        var rowchanged = await cmd.ExecuteNonQueryAsync();
        connection.Close();
        return rowchanged;
    }
    public async Task<int> DeleteAsync(NpgsqlConnection connection, string table, IEnumerable<DBEntityObject> entries)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"DELETE FROM {table} WHERE {string.Join(", ", entries.Select(x => $"{x.Column} = '{x.Value}'"))};", connection);
        var rowchanged = await cmd.ExecuteNonQueryAsync();
        connection.Close();
        return rowchanged;
    }
    public async Task<int> DeleteAsync(NpgsqlConnection connection, string table, DBEntityObject entry)
    {
        await OpenConnectionAsync(connection);
        var cmd = new NpgsqlCommand($"DELETE FROM {table} WHERE {entry.Column} = '{entry.Value}'", connection);
        var rowchanged = await cmd.ExecuteNonQueryAsync();
        connection.Close();
        return rowchanged;
    }
    private async Task OpenConnectionAsync(NpgsqlConnection connection)
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