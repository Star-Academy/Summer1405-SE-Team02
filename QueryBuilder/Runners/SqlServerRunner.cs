using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QueryBuilder.Models;

namespace QueryBuilder.Runners
{
    public class SqlServerRunner
    {
        public void Execute(SqlResult sqlResult)
        {
            var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Error: SQLSERVER_CONNECTION_STRING environment variable is not set.");
                return;
            }

            Console.WriteLine("--- Executing on SQL Server ---");
            Console.WriteLine($"Generated SQL: {sqlResult.Sql}");

            using (var sqlServerConnection = new SqlConnection(connectionString))
            {
                sqlServerConnection.Open();

                using (var sqlCommand = new SqlCommand(sqlResult.Sql, sqlServerConnection))
                {
                    AddParameters(sqlCommand, sqlResult.Bindings);
                    ExecuteAndPrintResults(sqlCommand);
                }
            }
        }

        private void AddParameters(SqlCommand sqlCommand, Dictionary<string, string> bindings)
        {
            foreach (var parameter in bindings)
            {
                var sqlParameter = new SqlParameter
                {
                    ParameterName = parameter.Key
                };

                if (bool.TryParse(parameter.Value, out var boolValue))
                {
                    sqlParameter.Value = boolValue;
                }
                else if (int.TryParse(parameter.Value, out var intValue))
                {
                    sqlParameter.Value = intValue;
                }
                else
                {
                    sqlParameter.Value = parameter.Value;
                }

                sqlCommand.Parameters.Add(sqlParameter);
            }
        }

        private void ExecuteAndPrintResults(SqlCommand sqlCommand)
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