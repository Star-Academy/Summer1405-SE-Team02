using Microsoft.Data.SqlClient;
using Npgsql;
using QueryBuilderWebApplication.Abstractions;
using QueryBuilderWebApplication.Models;
using SqlKata.Compilers;
using SqlKata.Execution;
using QueryBuilderWebApplication.Exceptions;

namespace QueryBuilderWebApplication.Services;

public class DatabaseResolver : IDatabaseResolver
{
    private readonly IConfiguration _configuration;

    public DatabaseResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseResolverResult Resolve(string? databaseName)
    {
        switch (databaseName?.Trim().ToLower())
        {
            case "postgres":
                return new DatabaseResolverResult(
                    new QueryFactory(
                        new NpgsqlConnection(_configuration.GetConnectionString("Postgres")),
                        new PostgresCompiler()));

            case "sqlserver":
                return new DatabaseResolverResult(
                    new QueryFactory(
                        new SqlConnection(_configuration.GetConnectionString("SqlServer")),
                        new SqlServerCompiler()));

            default:
                throw new InvalidDatabaseException(databaseName ?? string.Empty);
        }
    }
}