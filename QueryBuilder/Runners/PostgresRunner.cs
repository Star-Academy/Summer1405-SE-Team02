using System;
using System.Collections.Generic;
using Npgsql;
using QueryBuilder.Abstractions;
using QueryBuilder.Models;
using System.Diagnostics.CodeAnalysis;

namespace QueryBuilder.Runners
{
    internal sealed class PostgresRunner : IQueryRunner
    {
        private readonly string _connectionString;
        private readonly IParameterBinder _parameterBinder;

        public PostgresRunner(string connectionString, IParameterBinder parameterBinder)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _parameterBinder = parameterBinder ?? throw new ArgumentNullException(nameof(parameterBinder));
        }

        [ExcludeFromCodeCoverage]
        public List<Dictionary<string, string>> Execute(CompiledQuery compiledQuery)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(compiledQuery.RawSql, connection))
                {
                    _parameterBinder.BindParameters(command, compiledQuery.Bindings);

                    using (var reader = command.ExecuteReader())
                    {
                        return ResultSetMapper.Map(reader);
                    }
                }
            }
        }
    }
}