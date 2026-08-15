using Microsoft.Data.SqlClient;
using Npgsql;
using QueryBuilderWebApplication.Abstractions;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace QueryBuilderWebApplication.Services;

public class DbFactory : IDbFactory
{
    private readonly IConfiguration _configuration;

    public DbFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public QueryFactory Create(string databaseName)
    {
        switch (databaseName.Trim().ToLower())
        {
            case "postgres":
                return new QueryFactory(
                    new NpgsqlConnection(_configuration.GetConnectionString("Postgres")),
                    new PostgresCompiler());

            case "sqlserver":
                return new QueryFactory(
                    new SqlConnection(_configuration.GetConnectionString("SqlServer")),
                    new SqlServerCompiler());

            default:
                throw new InvalidDatabaseException(databaseName);
        }
    }
}