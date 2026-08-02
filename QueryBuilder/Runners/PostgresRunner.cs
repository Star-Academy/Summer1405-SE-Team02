using System;
using System.Collections.Generic;
using Npgsql;
using QueryBuilder.Models;

namespace QueryBuilder.Runners
{
    public class PostgresRunner
    {
        public void Execute(SqlResult sqlResult)
        {
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Error: POSTGRES_CONNECTION_STRING environment variable is not set.");
                return;
            }

            Console.WriteLine("--- Executing on PostgreSQL ---");
            Console.WriteLine($"Generated SQL: {sqlResult.Sql}");

            using (var postgresConnection = new NpgsqlConnection(connectionString))
            {
                postgresConnection.Open();

                using (var sqlCommand = new NpgsqlCommand(sqlResult.Sql, postgresConnection))
                {
                    AddParameters(sqlCommand, sqlResult.Bindings);
                    ExecuteAndPrintResults(sqlCommand);
                }
            }
        }

        private void AddParameters(NpgsqlCommand sqlCommand, Dictionary<string, string> bindings)
        {
            foreach (var parameter in bindings)
            {
                var npgsqlParameter = new NpgsqlParameter();

                if (bool.TryParse(parameter.Value, out var boolValue))
                {
                    npgsqlParameter.Value = boolValue;
                }
                else if (int.TryParse(parameter.Value, out var intValue))
                {
                    npgsqlParameter.Value = intValue;
                }
                else
                {
                    npgsqlParameter.Value = parameter.Value;
                }

                sqlCommand.Parameters.Add(npgsqlParameter);
            }
        }

        private void ExecuteAndPrintResults(NpgsqlCommand sqlCommand)
        {
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    for (var index = 0; index < dataReader.FieldCount; index++)
                    {
                        var columnName = dataReader.GetName(index);
                        var columnValue = dataReader.GetValue(index).ToString();

                        Console.Write($"{columnName}: {columnValue} | ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}