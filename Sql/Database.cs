using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Sepidar.Sql
{
    public class Database
    {
        string connectionString;

        private Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static Database Open(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "context connection=true;";
            }
            return new Database(connectionString);
        }

        public void Run(string sql, int? timeoutInSeconds = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = sql;
                if (timeoutInSeconds.HasValue)
                {
                    command.CommandTimeout = timeoutInSeconds.Value;
                }
                command.ExecuteNonQuery();
            }
        }

        public DataTable Get(string sql)
        {
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            using (var adapter = new SqlDataAdapter())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = sql;
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
    }
}
