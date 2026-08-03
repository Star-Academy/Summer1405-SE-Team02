using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QueryBuilder.Abstractions;
using QueryBuilder.Models;

namespace QueryBuilder.Runners
{
    public class SqlServerRunner : IQueryRunner
    {
        private readonly string _connectionString;
        private readonly IParameterBinder _parameterBinder;

        public SqlServerRunner(string connectionString, IParameterBinder parameterBinder)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _parameterBinder = parameterBinder ?? throw new ArgumentNullException(nameof(parameterBinder));
        }

        public List<Dictionary<string, string>> Execute(CompiledQuery compiledQuery)
        {
            var results = new List<Dictionary<string, string>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(compiledQuery.RawSql, connection))
                {
                    _parameterBinder.BindParameters(command, compiledQuery.Bindings);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, string>();
                            for (var index = 0; index < reader.FieldCount; index++)
                            {
                                var columnName = reader.GetName(index);
                                var columnValue = reader.GetValue(index)?.ToString() ?? string.Empty;
                                row.Add(columnName, columnValue);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            return results;
        }
    }
}